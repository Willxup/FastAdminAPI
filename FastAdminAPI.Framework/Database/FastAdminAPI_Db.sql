/*
 Navicat Premium Data Transfer

 Source Server         : FastAdminAPIDb
 Source Server Type    : MySQL
 Source Server Version : 80021 (8.0.21)
 Source Host           : localhost:3306
 Source Schema         : FastAdminAPIDb

 Target Server Type    : MySQL
 Target Server Version : 80021 (8.0.21)
 File Encoding         : 65001

 Date: 02/01/2024 17:59:56
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for S01_User
-- ----------------------------
DROP TABLE IF EXISTS `S01_User`;
CREATE TABLE `S01_User`  (
  `S01_UserId` bigint NOT NULL AUTO_INCREMENT COMMENT '用户Id',
  `S01_Account` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '用户账号',
  `S01_Password` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '用户密码',
  `S01_AccountStatus` tinyint NOT NULL DEFAULT 0 COMMENT '账号状态 0启用 1禁用',
  `S01_IsDelete` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除 0否 1是',
  `S01_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S01_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S01_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S01_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S01_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S01_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  `S01_DeleteId` bigint NULL DEFAULT NULL COMMENT '删除者Id',
  `S01_DeleteBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '删除者名称',
  `S01_DeleteTime` datetime NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`S01_UserId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '用户表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S02_Module
-- ----------------------------
DROP TABLE IF EXISTS `S02_Module`;
CREATE TABLE `S02_Module`  (
  `S02_ModuleId` bigint NOT NULL AUTO_INCREMENT COMMENT '模块Id',
  `S02_ParentModuleId` bigint NULL DEFAULT NULL COMMENT '父模块Id',
  `S02_Kind` tinyint NOT NULL COMMENT '属性 0菜单 1页面 2按钮 3列表 9其他',
  `S02_ModuleName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '名称',
  `S02_Priority` int NULL DEFAULT NULL COMMENT '优先级',
  `S02_Depth` int NULL DEFAULT NULL COMMENT '深度 0根菜单 1一级菜单 2二级菜单 （以此类推）',
  `S02_FrontRoute` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '前端路由 Url路径/控件Id',
  `S02_Logo` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '图标',
  `S02_BackInterface` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '后端接口 调用的接口，多个以|号分隔',
  `S02_CornerMark` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '角标 每一级4位数字',
  `S02_IsDelete` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除 0否 1是',
  `S02_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S02_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S02_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S02_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S02_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S02_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  `S02_DeleteId` bigint NULL DEFAULT NULL COMMENT '删除者Id',
  `S02_DeleteBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '删除者名称',
  `S02_DeleteTime` datetime NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`S02_ModuleId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '模块' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S03_Role
-- ----------------------------
DROP TABLE IF EXISTS `S03_Role`;
CREATE TABLE `S03_Role`  (
  `S03_RoleId` bigint NOT NULL AUTO_INCREMENT COMMENT '角色Id',
  `S03_ParentRoleId` bigint NULL DEFAULT NULL COMMENT '父角色Id',
  `S03_RoleName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '名称',
  `S03_CornerMark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '角标 每一级4位数字',
  `S03_IsDelete` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除 0否 1是',
  `S03_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S03_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S03_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S03_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S03_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S03_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  `S03_DeleteId` bigint NULL DEFAULT NULL COMMENT '删除者Id',
  `S03_DeleteBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '删除者名称',
  `S03_DeleteTime` datetime NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`S03_RoleId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '角色' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S04_RolePermission
-- ----------------------------
DROP TABLE IF EXISTS `S04_RolePermission`;
CREATE TABLE `S04_RolePermission`  (
  `S04_RolePermissionId` bigint NOT NULL AUTO_INCREMENT COMMENT '角色权限Id',
  `S02_ModuleId` bigint NOT NULL COMMENT '模块Id',
  `S03_RoleId` bigint NOT NULL COMMENT '角色Id',
  `S04_PermissionName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '权限名称',
  `S04_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S04_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S04_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S04_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S04_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S04_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  PRIMARY KEY (`S04_RolePermissionId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '角色权限' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S05_Department
-- ----------------------------
DROP TABLE IF EXISTS `S05_Department`;
CREATE TABLE `S05_Department`  (
  `S05_DepartId` bigint NOT NULL AUTO_INCREMENT COMMENT '部门Id',
  `S05_DepartName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '部门名称',
  `S05_ParentDepartId` bigint NULL DEFAULT NULL COMMENT '上级部门Id',
  `S05_CornerMark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '角标 每一级4位数字',
  `S05_Property` bigint NULL DEFAULT NULL COMMENT '部门属性 S99_Code',
  `S05_Label` varchar(5) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '部门标签 -1无 0线索 1客户 2代理，多个以,号分隔',
  `S05_Priority` int NULL DEFAULT NULL COMMENT '优先级',
  `S05_IsDelete` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除 0否 1是',
  `S05_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S05_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S05_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S05_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S05_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S05_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  `S05_DeleteId` bigint NULL DEFAULT NULL COMMENT '删除者Id',
  `S05_DeleteBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '删除者名称',
  `S05_DeleteTime` datetime NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`S05_DepartId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '部门' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S06_Post
-- ----------------------------
DROP TABLE IF EXISTS `S06_Post`;
CREATE TABLE `S06_Post`  (
  `S06_PostId` bigint NOT NULL AUTO_INCREMENT COMMENT '岗位Id',
  `S06_PostName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '岗位名称',
  `S06_ParentPostId` bigint NULL DEFAULT NULL COMMENT '上级岗位Id',
  `S05_DepartId` bigint NOT NULL COMMENT '部门Id',
  `S06_CornerMark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '角标 每一级4位数字',
  `S06_MaxEmployeeNums` int NOT NULL DEFAULT 0 NULL COMMENT '岗位编制',
  `S06_Responsibility` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '岗位职责',
  `S06_AbilityDemand` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '能力需求',
  `S06_IsDelete` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除 0否 1是',
  `S06_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S06_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S06_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S06_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S06_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S06_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  `S06_DeleteId` bigint NULL DEFAULT NULL COMMENT '删除者Id',
  `S06_DeleteBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '删除者名称',
  `S06_DeleteTime` datetime NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`S06_PostId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '岗位' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S07_Employee
-- ----------------------------
DROP TABLE IF EXISTS `S07_Employee`;
CREATE TABLE `S07_Employee`  (
  `S07_EmployeeId` bigint NOT NULL AUTO_INCREMENT COMMENT '员工Id',
  `S01_UserId` bigint NULL DEFAULT NULL COMMENT '用户Id',
  `S10_CompanyId` bigint NULL DEFAULT NULL COMMENT '企业Id',
  `S07_QyUserId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '企业微信UserId',
  `S07_Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '姓名',
  `S07_Phone` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '手机',
  `S07_Gender` tinyint NULL DEFAULT NULL COMMENT '性别 0男 1女',
  `S07_Avatar` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '头像',
  `S07_Email` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '邮箱',
  `S07_Kind` tinyint NULL DEFAULT NULL COMMENT '类别 0全职 1兼职',
  `S07_Status` tinyint NULL DEFAULT NULL COMMENT '状态 0正式 1实习 2离职',
  `S07_Bio` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '个人简介',
  `S07_HireDate` datetime NULL DEFAULT NULL COMMENT '入职日期',
  `S07_TrialPeriodDate` datetime NULL DEFAULT NULL COMMENT '试用期日期',
  `S07_ConfirmationDate` datetime NULL DEFAULT NULL COMMENT '转正日期',
  `S07_TerminationDate` datetime NULL DEFAULT NULL COMMENT '离职日期',
  `S07_IsDelete` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除 0否 1是',
  `S07_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S07_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S07_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S07_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S07_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S07_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  `S07_DeleteId` bigint NULL DEFAULT NULL COMMENT '删除者Id',
  `S07_DeleteBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '删除者名称',
  `S07_DeleteTime` datetime NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`S07_EmployeeId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '员工' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S08_EmployeePost
-- ----------------------------
DROP TABLE IF EXISTS `S08_EmployeePost`;
CREATE TABLE `S08_EmployeePost`  (
  `S08_EmployeePostId` bigint NOT NULL AUTO_INCREMENT COMMENT '员工岗位Id',
  `S07_EmployeeId` bigint NOT NULL COMMENT '员工Id',
  `S06_PostId` bigint NOT NULL COMMENT '岗位Id',
  `S08_IsMainPost` tinyint NOT NULL DEFAULT 0 COMMENT '是否为主岗位 0否 1是',
  `S05_DepartId` bigint NOT NULL COMMENT '部门Id',
  `S08_IsDelete` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除 0否 1是',
  `S08_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S08_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S08_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S08_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S08_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S08_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  `S08_DeleteId` bigint NULL DEFAULT NULL COMMENT '删除者Id',
  `S08_DeleteBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '删除者名称',
  `S08_DeleteTime` datetime NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`S08_EmployeePostId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '员工岗位表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S09_UserPermission
-- ----------------------------
DROP TABLE IF EXISTS `S09_UserPermission`;
CREATE TABLE `S09_UserPermission`  (
  `S09_UserPermissionId` bigint NOT NULL AUTO_INCREMENT COMMENT '用户权限Id',
  `S01_UserId` bigint NOT NULL COMMENT '用户Id',
  `S09_PermissionType` tinyint NOT NULL COMMENT '权限类型 0角色 1用户',
  `S09_CommonId` bigint NOT NULL COMMENT '通用Id 角色Id/模块Id',
  `S09_PermissionName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '权限名称',
  `S09_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S09_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S09_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S09_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S09_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S09_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  PRIMARY KEY (`S09_UserPermissionId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '用户权限' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S10_DataPermission
-- ----------------------------
DROP TABLE IF EXISTS `S10_DataPermission`;
CREATE TABLE `S10_DataPermission`  (
  `S10_DataPermissionId` bigint NOT NULL AUTO_INCREMENT COMMENT '数据权限Id',
  `S07_EmployeeId` bigint NOT NULL COMMENT '员工Id',
  `S05_DepartIds` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '部门Id集合 多个以,分隔',
  `S10_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S10_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S10_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S10_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S10_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S10_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  PRIMARY KEY (`S10_DataPermissionId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '数据权限' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S11_CheckProcess
-- ----------------------------
DROP TABLE IF EXISTS `S11_CheckProcess`;
CREATE TABLE `S11_CheckProcess`  (
  `S11_CheckProcessId` bigint NOT NULL AUTO_INCREMENT COMMENT '审批流程Id',
  `S11_ApproveType` tinyint NOT NULL COMMENT '审批类型 0直接上级 1指定人员 2自选 3上级+指定人员 4上级+指定人员+金额',
  `S99_ApplicationType` bigint NULL DEFAULT NULL COMMENT '申请类型 关联S99_Code表',
  `S07_Applicants` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '申请人 使用,隔开',
  `S07_Approvers` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '审核人 使用,隔开',
  `S07_CarbonCopies` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '抄送人 使用,隔开',
  `S11_Amounts` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '审批金额上限 使用,隔开',
  `S11_Remark` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '备注',
  `S11_IsDelete` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除 0否 1是',
  `S11_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S11_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S11_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S11_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S11_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S11_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  `S11_DeleteId` bigint NULL DEFAULT NULL COMMENT '删除者Id',
  `S11_DeleteBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '删除者名称',
  `S11_DeleteTime` datetime NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`S11_CheckProcessId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '审批流程' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S12_Check
-- ----------------------------
DROP TABLE IF EXISTS `S12_Check`;
CREATE TABLE `S12_Check`  (
  `S12_CheckId` bigint NOT NULL AUTO_INCREMENT COMMENT '审批Id',
  `S12_ApplicationId` bigint NOT NULL COMMENT '申请Id 与业务数据关联',
  `S11_CheckProcessId` bigint NOT NULL COMMENT '审批流程Id',
  `S12_ApplicationCategory` tinyint NOT NULL COMMENT '申请类别 0项目管理 1院校管理 2营销管理 3线索管理 4客户管理',
  `S99_ApplicationType` bigint NOT NULL COMMENT '申请类型 关联S99_Code表',
  `S07_ApproverId` bigint NOT NULL COMMENT '审批人Id',
  `S12_ApproverName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '审批人名称',
  `S12_ApproversData` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '审批人json',
  `S12_CarbonCopiesData` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '抄送人json',
  `S12_CommonDataContent` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '公有数据内容json',
  `S12_PrivateDataContent` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '私有数据内容json',
  `S12_IsFinishCheck` tinyint NOT NULL DEFAULT 0 COMMENT '是否完成审批 0否 1是',
  `S12_ApplicationInfo` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '申请详情',
  `S12_Reason` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '申请理由',
  `S12_Remark` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '备注',
  `S12_IsDelete` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除 0否 1是',
  `S12_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S12_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S12_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S12_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S12_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S12_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  `S12_DeleteId` bigint NULL DEFAULT NULL COMMENT '删除者Id',
  `S12_DeleteBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '删除者名称',
  `S12_DeleteTime` datetime NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`S12_CheckId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '审批' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S13_CheckRecords
-- ----------------------------
DROP TABLE IF EXISTS `S13_CheckRecords`;
CREATE TABLE `S13_CheckRecords`  (
  `S13_CheckRecordId` bigint NOT NULL AUTO_INCREMENT COMMENT '审批记录Id',
  `S12_CheckId` bigint NOT NULL COMMENT '审批Id',
  `S07_ApproverId` bigint NOT NULL COMMENT '审批人Id',
  `S13_ApproverName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '审批人名称',
  `S13_ApprovalTime` datetime NULL DEFAULT NULL COMMENT '审批日期',
  `S13_IsApprove` tinyint NOT NULL COMMENT '是否通过 0否 1是',
  `S13_Reason` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '理由',
  `S13_Remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '备注',
  `S13_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S13_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S13_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S13_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S13_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S13_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  PRIMARY KEY (`S13_CheckRecordId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '审批记录表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for S98_RegionInfo
-- ----------------------------
DROP TABLE IF EXISTS `S98_RegionInfo`;
CREATE TABLE `S98_RegionInfo`  (
  `S98_REGION_ID` bigint NOT NULL AUTO_INCREMENT COMMENT '区域Id',
  `S98_REGION_CODE` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '区域Code',
  `S98_REGION_NAME` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '区域名称',
  `S98_PARENT_ID` bigint NOT NULL COMMENT '父级Id',
  `S98_REGION_LEVEL` bigint NOT NULL COMMENT '区域等级',
  `S98_REGION_ORDER` bigint NOT NULL COMMENT '区域排序',
  `S98_REGION_NAME_EN` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '区域英文名称',
  `S98_REGION_SHORTNAME_EN` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '区域英文短名',
  PRIMARY KEY (`S98_REGION_ID`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for S99_Code
-- ----------------------------
DROP TABLE IF EXISTS `S99_Code`;
CREATE TABLE `S99_Code`  (
  `S99_CodeId` bigint NOT NULL AUTO_INCREMENT COMMENT '系统字典Id',
  `S99_GroupCode` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '分组代号',
  `S99_GroupName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '分组名称',
  `S99_Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '字典名称',
  `S99_ParentCodeId` bigint NULL DEFAULT NULL COMMENT '父级Id',
  `S99_SeqNo` int NULL DEFAULT NULL COMMENT '序号 组内排序使用',
  `S99_SysFlag` tinyint NOT NULL DEFAULT 0 COMMENT '系统标记 0系统 1用户',
  `S99_IsDelete` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除 0否 1是',
  `S99_CreateId` bigint NOT NULL COMMENT '创建者Id',
  `S99_CreateBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '创建者名称',
  `S99_CreateTime` datetime NOT NULL COMMENT '创建日期',
  `S99_ModifyId` bigint NULL DEFAULT NULL COMMENT '更新者Id',
  `S99_ModifyBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '更新者名称',
  `S99_ModifyTime` datetime NULL DEFAULT NULL COMMENT '更新日期',
  `S99_DeleteId` bigint NULL DEFAULT NULL COMMENT '删除者Id',
  `S99_DeleteBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '删除者名称',
  `S99_DeleteTime` datetime NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`S99_CodeId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '系统字典' ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
