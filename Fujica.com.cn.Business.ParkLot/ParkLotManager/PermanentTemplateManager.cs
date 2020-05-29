/***************************************************************************************
 * *
 * *        File Name        : ParkLotManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 收费设置(模板)管理 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    ///  收费设置(模板)管理.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 新增注释信息. llp <br/> 
    /// </remarks>  
    partial class ParkLotManager : IBaseBusiness
    {
        /// <summary>
        /// 设置固定车延期模板
        /// </summary>
        /// <returns></returns>
        public bool SetPermanentTemplate(PermanentTemplateModel model)
        {
            PermanentTemplateModel content = GetPermanentTemplate(model.CarTypeGuid);
            //是新增还是编辑操作
            bool isEdit = false;
            if (content != null)
            {
                //验证数据库值和传输内容是否一致
                if (content.ProjectGuid != model.ProjectGuid)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_PARAM_ERROR_PROJECTGUID.GetDesc();
                    return false;
                }
                if (content.ParkCode != model.ParkCode)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_PARAM_ERROR_PARKINGCODE.GetDesc();
                    return false;
                }
                isEdit = true;
            }

            bool flag = _iPermanentTemplateContext.SetPermanentTemplate(model); 
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_BILLING.GetDesc();
                return false; //数据库不成功就不要往下执行了
            }

            //将数据同步到主平台Fujica Api
            bool toFujicaResult = false;
            if (isEdit)
                toFujicaResult = EditTemplateDataToFujica(model);
            else
                toFujicaResult = AddTemplateDataToFujica(model);
            if (!toFujicaResult)
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_BILLING_TOFUJICA.GetDesc();
            return toFujicaResult;
        }

        /// <summary>
        /// 删除固定车延期模板
        /// </summary>
        /// <returns></returns>
        public bool DeletePermanentTemplate(PermanentTemplateModel model)
        {
            bool flag = _iPermanentTemplateContext.DeletePermanentTemplate(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_DELETE_BILLING.GetDesc();
                return false; //数据库不成功就不要往下执行了
            }

            //将数据同步到主平台Fujica Api
            bool toFujicaResult = DeleteTemplateDataToFujica(model);
            if (!toFujicaResult)
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_BILLING_TOFUJICA.GetDesc();
            return toFujicaResult;
        }

        /// <summary>
        /// 某车场所有固定车延期模板
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<PermanentTemplateModel> AllPermanentTemplate(string parkcode)
        {
            List<PermanentTemplateModel> list = _iPermanentTemplateContext.AllPermanentTemplate(parkcode);
            return list;
        }

        /// <summary>
        /// 获取固定车延期模板
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public PermanentTemplateModel GetPermanentTemplate(string cartypeguid)
        {
            PermanentTemplateModel model = _iPermanentTemplateContext.GetPermanentTemplate(cartypeguid);
            return model;
        }

        #region 内部业务私有方法

        /// <summary>
        /// 新增-模板数据同步到主平台Fujica
        /// </summary>
        /// <param name="model">固定车延期模板</param>
        /// <returns></returns>
        private bool AddTemplateDataToFujica(PermanentTemplateModel model)
        {
            return SetTemplateDataToFujica(model, 1);
        }

        /// <summary>
        /// 编辑-模板数据同步到主平台Fujica
        /// </summary>
        /// <param name="model">固定车延期模板</param>
        /// <returns></returns>
        private bool EditTemplateDataToFujica(PermanentTemplateModel model)
        {
            return SetTemplateDataToFujica(model, 2);
        }

        /// <summary>
        /// 删除-模板数据同步到主平台Fujica
        /// </summary>
        /// <param name="model">固定车延期模板</param>
        /// <returns></returns>
        private bool DeleteTemplateDataToFujica(PermanentTemplateModel model)
        {
            return SetTemplateDataToFujica(model, 3);
        }

        /// <summary>
        /// 新增/编辑-模板数据同步到主平台Fujica
        /// </summary>
        /// <param name="model">固定车延期模板</param>
        /// <param name="actionType">新增:1 修改:2 删除:3</param>
        /// <returns></returns>
        private bool SetTemplateDataToFujica(PermanentTemplateModel model, int actionType)
        {
            //1、根据不同的计费方式将数据进行格式化
            string templateStr = TemplateDataFormatStr(model);

            //2、格式化后数据同步到主平台
            if (!string.IsNullOrEmpty(templateStr))
            {
                return TemplateDataToFujica(model.CarTypeGuid, actionType, templateStr);
            }
            return false;
        }

        /// <summary>
        /// 模板数据格式化
        /// </summary>
        /// <param name="model">固定车延期模板</param>
        /// <returns></returns>
        private string TemplateDataFormatStr(PermanentTemplateModel model)
        {
            //示例：MAD00001000M00010000Y00000000
            //说明：M:固定卡标识 A: 卡类(B、C、D)  D00000000: 天金额(单位: 分)  M00000000: 月金额(单位: 分)  Y00000000: 年金额(单位: 分)
            //*系统目前只控制月份字段内容
            string templateStr = "MA";
            string dayStr = "D00000000";
            string monthStr = "M" + Convert.ToInt32(model.Amount * 100).ToString().PadLeft(8, '0');
            string yearStr = "Y00000000";
            templateStr = templateStr + dayStr + monthStr + yearStr;
            return templateStr;
        }

        /// <summary>
        /// 模板数据同步到主平台Fujica
        /// </summary>
        /// <param name="carTypeGuid">车类唯一编码</param>
        /// <param name="parkingCode">停车场编号</param>
        /// <param name="actionType">新增:1 修改:2 删除:3</param>
        /// <param name="templateStr">格式化模板</param>
        /// <returns></returns>
        private bool TemplateDataToFujica(string carTypeGuid, int actionType, string templateStr)
        {
            CarTypeModel carTypeModel = _iCarTypeContext.GetCarType(carTypeGuid);
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/CalculationCost/";
            switch (actionType)
            {
                case 1://新增api
                    servername += "ApiAddMonthCardTemplate";
                    break;
                case 2://修改api
                    servername += "ApiUpdateMonthCardTemplate";
                    break;
                case 3://删除api
                    servername += "ApiDeleteTemplate";
                    break;
            }

            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ParkingCode"] = carTypeModel.ParkCode;
            dicParam["smallparkingCode"] = "0";
            dicParam["CarType"] = carTypeModel.Idx;
            if (actionType != 3)
            {//删除api不需要这2个字段
                dicParam["CarName"] = carTypeModel.CarTypeName;
                dicParam["Template"] = templateStr;
            }
            //返回fujica api计费模板添加、修改、删除请求结果
            return requestFujica.RequestInterfaceV2(servername, dicParam);

        }

        #endregion
    }
}
