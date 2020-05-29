/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-11-08 10:43:10
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_billingtemplate
-- ----------------------------
DROP TABLE IF EXISTS `t_billingtemplate`;
CREATE TABLE `t_billingtemplate` (
  `projectGuid` varchar(32) NOT NULL,
  `parkCode` varchar(20) NOT NULL,
  `carTypeGuid` varchar(32) NOT NULL COMMENT '对应的车类型guid',
  `chargeMode` int(11) DEFAULT NULL COMMENT '计费方式共10种1=小时算费 2=分段算费 3=深圳算费 4=按半小时算费 5=简易分段算费 6=分段按小时算费 7=半小时算费(可分段) 8=分段按半小时算费 9=新分段收费标准10=分段按15分钟算费',
  `templateJson` text COMMENT '模板实体json字符串',
  PRIMARY KEY (`carTypeGuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
