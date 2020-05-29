/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2019-01-21 09:41:52
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_opengatereason
-- ----------------------------
DROP TABLE IF EXISTS `t_opengatereason`;
CREATE TABLE `t_opengatereason` (
  `projectGuid` varchar(32) NOT NULL,
  `guid` varchar(32) NOT NULL,
  `openType` int(11) DEFAULT NULL COMMENT '开闸类型 0=手动 1=免费',
  `reasonRemark` text COMMENT '开闸原因注明',
  PRIMARY KEY (`guid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
