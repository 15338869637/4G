/***************************************************************************************
 * *
 * *        File Name        : ParkLotManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21
 * *        Functional Description  : 车辆进出逻辑模块
 * *        Remark           :  
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 车类管理.
    /// </summary>
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/>  
    partial class ParkLotManager
    {
        /// <summary>
        /// 添加车类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddNewCarType(CarTypeModel model)
        {
            bool flag = _iCarTypeContext.AddCarType(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARTYPE.GetDesc();
                return false;
            }
            return flag;
        }

        /// <summary>
        /// 修改车类
        /// </summary>
        /// <returns></returns>
        public bool ModifyCarType(CarTypeModel model)
        {
            bool flag = _iCarTypeContext.ModifyCarType(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARTYPE.GetDesc();
                return false;
            }
            return flag;
        }

        /// <summary>
        /// 删除车类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteCarType(CarTypeModel model)
        {
            bool flag = false;
            //删除计费模板
            if (model.CarType == CarTypeEnum.TempCar || model.CarType == CarTypeEnum.ValueCar)
            {
                //如果当前车类为临时车或者储值车
                //判断该车类是否已创建计费模板
                BillingTemplateModel billingModel = GetBillingTemplate(model.Guid);
                if (billingModel != null)
                {
                    //计费模板存在则需要先删除计费模板
                    flag = DeleteBillingTemplate(billingModel);
                    if (!flag) return flag;
                }
            }
            else
            {
                //删除固定车延期模板
                PermanentTemplateModel permanentModel = GetPermanentTemplate(model.Guid);
                if (permanentModel != null)
                {
                    //如果存在则删除固定车延期模板
                    flag = DeletePermanentTemplate(permanentModel);
                    if (!flag) return flag;
                }
            }

            flag = _iCarTypeContext.DeleteCarType(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_DELETE_CARTYPE.GetDesc();
                return false;
            }
            return flag;
        }

        /// <summary>
        /// 设置默认车类(临时车时用到)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool SetDefaultCarType(string guid)
        {
            bool flag = _iCarTypeContext.SetDefaultCarType(guid);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARTYPE.GetDesc();
                return false;
            }
            return flag;
        }

        /// <summary>
        /// 获取某车类
        /// </summary>
        /// <param name="guid">车类guid</param>
        /// <returns></returns>
        public CarTypeModel GetCarType(string guid)
        {
            return _iCarTypeContext.GetCarType(guid);
        }

        /// <summary>
        /// 某车场的所有车类
        /// </summary>
        /// <param name="parkcode">停车场编码</param>
        /// <returns></returns>
        public List<CarTypeModel> AllCarType(string parkcode,string projectguid)
        {
            return _iCarTypeContext.AllCarType(parkcode, projectguid);
        }



        /// <summary>
        /// 初始化新增车类 时租车、月租车、储值车、贵宾车
        /// 该接口目前仅在新建车场时候用
        /// </summary>
        /// <param name="projectGuid">项目guid</param>
        /// <param name="parkingCode">停车场编号</param>   
        /// <returns>初始化的车类集合</returns>
        public List<CarTypeModel> InitNewCarTypeAll(string projectGuid,string parkingCode)
        {
            List<CarTypeModel> carTypeList = new List<CarTypeModel>();
            #region 时租车ABCD

            CarTypeModel model = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "时租车A",
                Idx= "1A",
                CarType = CarTypeEnum.TempCar,
                DefaultType = true,
                Enable = true
            };
            CarTypeModel model2 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "时租车B",
                Idx = "1B",
                CarType = CarTypeEnum.TempCar,
                DefaultType = false,
                Enable = true
            };
            CarTypeModel model3 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "时租车C",
                Idx = "1C",
                CarType = CarTypeEnum.TempCar,
                DefaultType = false,
                Enable = true
            };
            CarTypeModel model4 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "时租车D",
                Idx = "1D",
                CarType = CarTypeEnum.TempCar,
                DefaultType = false,
                Enable = true
            };

            carTypeList.Add(model);
            carTypeList.Add(model2);
            carTypeList.Add(model3);
            carTypeList.Add(model4);

            AddNewCarType(model);
            AddNewCarType(model2);
            AddNewCarType(model3);
            AddNewCarType(model4);
            #endregion

            #region 储值车ABCD

            CarTypeModel model5 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "储值车A",
                Idx = "2A",
                CarType = CarTypeEnum.ValueCar,
                DefaultType = false,
                Enable = true
            };
            CarTypeModel model6 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "储值车B",
                Idx = "2B",
                CarType = CarTypeEnum.ValueCar,
                DefaultType = false,
                Enable = true
            };
            CarTypeModel model7 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "储值车C",
                Idx = "2C",
                CarType = CarTypeEnum.ValueCar,
                DefaultType = false,
                Enable = true
            };
            CarTypeModel model8 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "储值车D",
                Idx = "2D",
                CarType = CarTypeEnum.ValueCar,
                DefaultType = false,
                Enable = true
            };
            carTypeList.Add(model5);
            carTypeList.Add(model6);
            carTypeList.Add(model7);
            carTypeList.Add(model8);

            AddNewCarType(model5);
            AddNewCarType(model6);
            AddNewCarType(model7);
            AddNewCarType(model8);

            #endregion

            #region 月租车ABCD
            CarTypeModel model9 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "月租车A",
                Idx = "3A",
                CarType = CarTypeEnum.MonthCar,
                DefaultType = false,
                Enable = true
            };
            CarTypeModel model10 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "月租车B",
                Idx = "3B",
                CarType = CarTypeEnum.MonthCar,
                DefaultType = false,
                Enable = true
            };
            CarTypeModel model11 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "月租车C",
                Idx = "3C",
                CarType = CarTypeEnum.MonthCar,
                DefaultType = false,
                Enable = true
            };
            CarTypeModel model12 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "月租车D",
                Idx = "3D",
                CarType = CarTypeEnum.MonthCar,
                DefaultType = false,
                Enable = true
            };

            carTypeList.Add(model9);
            carTypeList.Add(model10);
            carTypeList.Add(model11);
            carTypeList.Add(model12);

            AddNewCarType(model9);
            AddNewCarType(model10);
            AddNewCarType(model11);
            AddNewCarType(model12);
            #endregion

            #region 贵宾车AB

            CarTypeModel model13 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "贵宾车A",
                Idx = "4A",
                CarType = CarTypeEnum.VIPCar,
                DefaultType = false,
                Enable = true
            };
            CarTypeModel model14 = new CarTypeModel()
            {
                ProjectGuid = projectGuid,
                ParkCode = parkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarTypeName = "贵宾车B",
                Idx = "4B",
                CarType = CarTypeEnum.VIPCar,
                DefaultType = false,
                Enable = true
            };

            carTypeList.Add(model13);
            carTypeList.Add(model14);

            AddNewCarType(model13);
            AddNewCarType(model14);

            #endregion

            return carTypeList;
        }
        
    }
}
