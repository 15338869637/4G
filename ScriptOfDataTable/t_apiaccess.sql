/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-11-05 15:50:29
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_apiaccess
-- ----------------------------
DROP TABLE IF EXISTS `t_apiaccess`;
CREATE TABLE `t_apiaccess` (
  `appid` varchar(50) NOT NULL,
  `secret` varchar(50) NOT NULL,
  `publickey` varchar(1000) NOT NULL,
  `privatekey` varchar(2000) NOT NULL,
  `privatekeypkcs8` varchar(2000) NOT NULL,
  `needverify` int(11) DEFAULT NULL COMMENT '是否需要验证,1 时间戳验证、2 签名验证、 4 接口权限验证、 8参数验证',
  `enable` int(11) DEFAULT '0' COMMENT '0=不能接入 1=可以接入',
  PRIMARY KEY (`appid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
