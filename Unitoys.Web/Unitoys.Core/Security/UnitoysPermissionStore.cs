using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.Core.Security
{
    public class UnitoysPermissionStore
    {
        //Permission

        #region User

        public const string Can_Add_User = "Can_Add_User";
        public const string Can_View_User = "Can_View_User";
        public const string Can_Modify_User = "Can_Modify_User";
        public const string Can_Delete_User = "Can_Delete_User";

        #endregion

        #region Package

        public const string Can_Add_Package = "Can_Add_Package";
        public const string Can_View_Package = "Can_View_Package";
        public const string Can_Modify_Package = "Can_Modify_Package";
        public const string Can_Delete_Package = "Can_Delete_Package";

        #endregion

        #region UserBill

        public const string Can_Add_UserBill = "Can_Add_UserBill";
        public const string Can_View_UserBill = "Can_View_UserBill";
        public const string Can_Modify_UserBill = "Can_Modify_UserBill";
        public const string Can_Delete_UserBill = "Can_Delete_UserBill";

        #endregion

        #region SpeakRecord

        public const string Can_Add_SpeakRecord = "Can_Add_SpeakRecord";
        public const string Can_View_SpeakRecord = "Can_View_SpeakRecord";
        public const string Can_Modify_SpeakRecord = "Can_Modify_SpeakRecord";
        public const string Can_Delete_SpeakRecord = "Can_Delete_SpeakRecord";

        #endregion

        #region SMS

        public const string Can_Add_SMS = "Can_Add_SMS";
        public const string Can_View_SMS = "Can_View_SMS";
        public const string Can_Modify_SMS = "Can_Modify_SMS";
        public const string Can_Delete_SMS = "Can_Delete_SMS";

        #endregion

        #region RolePermission

        public const string Can_Add_RolePermission = "Can_Add_RolePermission";
        public const string Can_View_RolePermission = "Can_View_RolePermission";
        public const string Can_Modify_RolePermission = "Can_Modify_RolePermission";
        public const string Can_Delete_RolePermission = "Can_Delete_RolePermission";

        #endregion

        #region Role

        public const string Can_Add_Role = "Can_Add_Role";
        public const string Can_View_Role = "Can_View_Role";
        public const string Can_Modify_Role = "Can_Modify_Role";
        public const string Can_Delete_Role = "Can_Delete_Role";

        #endregion

        #region Permission

        public const string Can_Add_Permission = "Can_Add_Permission";
        public const string Can_View_Permission = "Can_View_Permission";
        public const string Can_Modify_Permission = "Can_Modify_Permission";
        public const string Can_Delete_Permission = "Can_Delete_Permission";

        #endregion

        #region Payment

        public const string Can_Add_Payment = "Can_Add_Payment";
        public const string Can_View_Payment = "Can_View_Payment";
        public const string Can_Modify_Payment = "Can_Modify_Payment";
        public const string Can_Delete_Payment = "Can_Delete_Payment";

        #endregion

        #region Order

        public const string Can_Add_Order = "Can_Add_Order";
        public const string Can_View_Order = "Can_View_Order";
        public const string Can_Modify_Order = "Can_Modify_Order";
        public const string Can_Delete_Order = "Can_Delete_Order";

        #endregion

        #region ManageUser

        public const string Can_Add_ManageUser = "Can_Add_ManageUser";
        public const string Can_View_ManageUser = "Can_View_ManageUser";
        public const string Can_Modify_ManageUser = "Can_Modify_ManageUser";
        public const string Can_Delete_ManageUser = "Can_Delete_ManageUser";

        #endregion

        #region ManageUserRole

        public const string Can_Add_ManageUserRole = "Can_Add_ManageUserRole";
        public const string Can_View_ManageUserRole = "Can_View_ManageUserRole";
        public const string Can_Modify_ManageUserRole = "Can_Modify_ManageUserRole";
        public const string Can_Delete_ManageUserRole = "Can_Delete_ManageUserRole";

        #endregion

        #region Country

        public const string Can_Add_Country = "Can_Add_Country";
        public const string Can_View_Country = "Can_View_Country";
        public const string Can_Modify_Country = "Can_Modify_Country";
        public const string Can_Delete_Country = "Can_Delete_Country";

        #endregion

        #region CallTransferNum

        public const string Can_Add_CallTransferNum = "Can_Add_CallTransferNum";
        public const string Can_View_CallTransferNum = "Can_View_CallTransferNum";
        public const string Can_Modify_CallTransferNum = "Can_Modify_CallTransferNum";
        public const string Can_Delete_CallTransferNum = "Can_Delete_CallTransferNum";

        #endregion

        #region PaymentCard

        public const string Can_Add_PaymentCard = "Can_Add_PaymentCard";
        public const string Can_View_PaymentCard = "Can_View_PaymentCard";
        public const string Can_Modify_PaymentCard = "Can_Modify_PaymentCard";
        public const string Can_Delete_PaymentCard = "Can_Delete_PaymentCard";

        #endregion

        #region DeviceBracelet

        public const string Can_Add_DeviceBracelet = "Can_Add_DeviceBracelet";
        public const string Can_View_DeviceBracelet = "Can_View_DeviceBracelet";
        public const string Can_Modify_DeviceBracelet = "Can_Modify_DeviceBracelet";
        public const string Can_Delete_DeviceBracelet = "Can_Delete_DeviceBracelet";

        #endregion

        #region Feedback

        public const string Can_Add_Feedback = "Can_Add_Feedback";
        public const string Can_View_Feedback = "Can_View_Feedback";
        public const string Can_Modify_Feedback = "Can_Modify_Feedback";
        public const string Can_Delete_Feedback = "Can_Delete_Feedback";

        #endregion

        #region DeviceGoip

        public const string Can_Add_DeviceGoip = "Can_Add_DeviceGoip";
        public const string Can_View_DeviceGoip = "Can_View_DeviceGoip";
        public const string Can_Modify_DeviceGoip = "Can_Modify_DeviceGoip";
        public const string Can_Delete_DeviceGoip = "Can_Delete_DeviceGoip";

        #endregion

        #region Sport

        public const string Can_Add_Sport = "Can_Add_Sport";
        public const string Can_View_Sport = "Can_View_Sport";
        public const string Can_Modify_Sport = "Can_Modify_Sport";
        public const string Can_Delete_Sport = "Can_Delete_Sport";

        #endregion

        #region Banner

        public const string Can_Add_Banner = "Can_Add_Banner";
        public const string Can_View_Banner = "Can_View_Banner";
        public const string Can_Modify_Banner = "Can_Modify_Banner";
        public const string Can_Delete_Banner = "Can_Delete_Banner";

        #endregion

        #region PageShow

        public const string Can_Add_PageShow = "Can_Add_PageShow";
        public const string Can_View_PageShow = "Can_View_PageShow";
        public const string Can_Modify_PageShow = "Can_Modify_PageShow";
        public const string Can_Delete_PageShow = "Can_Delete_PageShow";

        #endregion

        #region Config

        public const string Can_Add_Config = "Can_Add_Config";
        public const string Can_View_Config = "Can_View_Config";
        public const string Can_Modify_Config = "Can_Modify_Config";
        public const string Can_Delete_Config = "Can_Delete_Config";

        #endregion

        #region OrderByZC

        public const string Can_Add_OrderByZC = "Can_Add_OrderByZC";
        public const string Can_View_OrderByZC = "Can_View_OrderByZC";
        public const string Can_Modify_OrderByZC = "Can_Modify_OrderByZC";
        public const string Can_Delete_OrderByZC = "Can_Delete_OrderByZC";

        #endregion

        #region ZCSelectionNumber

        public const string Can_Add_ZCSelectionNumber = "Can_Add_ZCSelectionNumber";
        public const string Can_View_ZCSelectionNumber = "Can_View_ZCSelectionNumber";
        public const string Can_Modify_ZCSelectionNumber = "Can_Modify_ZCSelectionNumber";
        public const string Can_Delete_ZCSelectionNumber = "Can_Delete_ZCSelectionNumber";

        #endregion

        #region EjoinDev

        public const string Can_Add_EjoinDev = "Can_Add_EjoinDev";
        public const string Can_View_EjoinDev = "Can_View_EjoinDev";
        public const string Can_Modify_EjoinDev = "Can_Modify_EjoinDev";
        public const string Can_Delete_EjoinDev = "Can_Delete_EjoinDev";

        #endregion

        #region EjoinDevSlot

        public const string Can_Add_EjoinDevSlot = "Can_Add_EjoinDevSlot";
        public const string Can_View_EjoinDevSlot = "Can_View_EjoinDevSlot";
        public const string Can_Modify_EjoinDevSlot = "Can_Modify_EjoinDevSlot";
        public const string Can_Delete_EjoinDevSlot = "Can_Delete_EjoinDevSlot";

        #endregion
        
        public static List<Tuple<string, string, int>> Properties
        {
            get
            {
                List<Tuple<string, string, int>> returnList = new List<Tuple<string, string, int>>();

                #region Property Define.

                Tuple<string, string, int> Can_Add_User = new Tuple<string, string, int>("Can_Add_User", "添加用户", 1);
                Tuple<string, string, int> Can_View_User = new Tuple<string, string, int>("Can_View_User", "查看用户", 1);
                Tuple<string, string, int> Can_Modify_User = new Tuple<string, string, int>("Can_Modify_User", "修改用户", 1);
                Tuple<string, string, int> Can_Delete_User = new Tuple<string, string, int>("Can_Delete_User", "删除用户", 1);

                Tuple<string, string, int> Can_Add_ManageUser = new Tuple<string, string, int>("Can_Add_ManageUser", "添加管理员", 2);
                Tuple<string, string, int> Can_View_ManageUser = new Tuple<string, string, int>("Can_View_ManageUser", "查看管理员", 2);
                Tuple<string, string, int> Can_Modify_ManageUser = new Tuple<string, string, int>("Can_Modify_ManageUser", "修改管理员", 2);
                Tuple<string, string, int> Can_Delete_ManageUser = new Tuple<string, string, int>("Can_Delete_ManageUser", "删除管理员", 2);

                Tuple<string, string, int> Can_Add_Role = new Tuple<string, string, int>("Can_Add_Role", "添加角色", 3);
                Tuple<string, string, int> Can_View_Role = new Tuple<string, string, int>("Can_View_Role", "查看角色", 3);
                Tuple<string, string, int> Can_Modify_Role = new Tuple<string, string, int>("Can_Modify_Role", "修改角色", 3);
                Tuple<string, string, int> Can_Delete_Role = new Tuple<string, string, int>("Can_Delete_Role", "删除角色", 3);

                Tuple<string, string, int> Can_Add_Permission = new Tuple<string, string, int>("Can_Add_Permission", "添加权限", 4);
                Tuple<string, string, int> Can_View_Permission = new Tuple<string, string, int>("Can_View_Permission", "查看权限", 4);
                Tuple<string, string, int> Can_Modify_Permission = new Tuple<string, string, int>("Can_Modify_Permission", "修改权限", 4);
                Tuple<string, string, int> Can_Delete_Permission = new Tuple<string, string, int>("Can_Delete_Permission", "删除权限", 4);

                Tuple<string, string, int> Can_Add_ManageUserRole = new Tuple<string, string, int>("Can_Add_ManageUserRole", "添加用户&角色", 5);
                Tuple<string, string, int> Can_View_ManageUserRole = new Tuple<string, string, int>("Can_View_ManageUserRole", "查看用户&角色", 5);
                Tuple<string, string, int> Can_Modify_ManageUserRole = new Tuple<string, string, int>("Can_Modify_ManageUserRole", "修改用户&角色", 5);
                Tuple<string, string, int> Can_Delete_ManageUserRole = new Tuple<string, string, int>("Can_Delete_ManageUserRole", "删除用户&角色", 5);

                Tuple<string, string, int> Can_Add_RolePermission = new Tuple<string, string, int>("Can_Add_RolePermission", "添加角色&权限", 6);
                Tuple<string, string, int> Can_View_RolePermission = new Tuple<string, string, int>("Can_View_RolePermission", "查看角色&权限", 6);
                Tuple<string, string, int> Can_Modify_RolePermission = new Tuple<string, string, int>("Can_Modify_RolePermission", "修改角色&权限", 6);
                Tuple<string, string, int> Can_Delete_RolePermission = new Tuple<string, string, int>("Can_Delete_RolePermission", "删除角色&权限", 6);

                Tuple<string, string, int> Can_Add_UserBill = new Tuple<string, string, int>("Can_Add_UserBill", "添加用户账单", 7);
                Tuple<string, string, int> Can_View_UserBill = new Tuple<string, string, int>("Can_View_UserBill", "查看用户账单", 7);
                Tuple<string, string, int> Can_Modify_UserBill = new Tuple<string, string, int>("Can_Modify_UserBill", "修改用户账单", 7);
                Tuple<string, string, int> Can_Delete_UserBill = new Tuple<string, string, int>("Can_Delete_UserBill", "删除用户账单", 7);

                Tuple<string, string, int> Can_Add_Order = new Tuple<string, string, int>("Can_Add_Order", "添加订单", 8);
                Tuple<string, string, int> Can_View_Order = new Tuple<string, string, int>("Can_View_Order", "查看订单", 8);
                Tuple<string, string, int> Can_Modify_Order = new Tuple<string, string, int>("Can_Modify_Order", "修改订单", 8);
                Tuple<string, string, int> Can_Delete_Order = new Tuple<string, string, int>("Can_Delete_Order", "删除订单", 8);

                Tuple<string, string, int> Can_Add_Package = new Tuple<string, string, int>("Can_Add_Package", "添加套餐", 9);
                Tuple<string, string, int> Can_View_Package = new Tuple<string, string, int>("Can_View_Package", "查看套餐", 9);
                Tuple<string, string, int> Can_Modify_Package = new Tuple<string, string, int>("Can_Modify_Package", "修改套餐", 9);
                Tuple<string, string, int> Can_Delete_Package = new Tuple<string, string, int>("Can_Delete_Package", "删除套餐", 9);

                Tuple<string, string, int> Can_Add_Payment = new Tuple<string, string, int>("Can_Add_Payment", "添加充值记录", 10);
                Tuple<string, string, int> Can_View_Payment = new Tuple<string, string, int>("Can_View_Payment", "查看充值记录", 10);
                Tuple<string, string, int> Can_Modify_Payment = new Tuple<string, string, int>("Can_Modify_Payment", "修改充值记录", 10);
                Tuple<string, string, int> Can_Delete_Payment = new Tuple<string, string, int>("Can_Delete_Payment", "删除充值记录", 10);

                Tuple<string, string, int> Can_Add_SMS = new Tuple<string, string, int>("Can_Add_SMS", "添加短信", 11);
                Tuple<string, string, int> Can_View_SMS = new Tuple<string, string, int>("Can_View_SMS", "查看短信", 11);
                Tuple<string, string, int> Can_Modify_SMS = new Tuple<string, string, int>("Can_Modify_SMS", "修改短信", 11);
                Tuple<string, string, int> Can_Delete_SMS = new Tuple<string, string, int>("Can_Delete_SMS", "删除短信", 11);

                Tuple<string, string, int> Can_Add_Country = new Tuple<string, string, int>("Can_Add_Country", "添加国家费率", 12);
                Tuple<string, string, int> Can_View_Country = new Tuple<string, string, int>("Can_View_Country", "查看国家费率", 12);
                Tuple<string, string, int> Can_Modify_Country = new Tuple<string, string, int>("Can_Modify_Country", "修改国家费率", 12);
                Tuple<string, string, int> Can_Delete_Country = new Tuple<string, string, int>("Can_Delete_Country", "删除国家费率", 12);

                Tuple<string, string, int> Can_Add_SpeakRecord = new Tuple<string, string, int>("Can_Add_SpeakRecord", "添加通话记录", 13);
                Tuple<string, string, int> Can_View_SpeakRecord = new Tuple<string, string, int>("Can_View_SpeakRecord", "查看通话记录", 13);
                Tuple<string, string, int> Can_Modify_SpeakRecord = new Tuple<string, string, int>("Can_Modify_SpeakRecord", "修改通话记录", 13);
                Tuple<string, string, int> Can_Delete_SpeakRecord = new Tuple<string, string, int>("Can_Delete_SpeakRecord", "删除通话记录", 13);

                Tuple<string, string, int> Can_Add_CallTransferNum = new Tuple<string, string, int>("Can_Add_CallTransferNum", "添加用户大号", 14);
                Tuple<string, string, int> Can_View_CallTransferNum = new Tuple<string, string, int>("Can_View_CallTransferNum", "查看用户大号", 14);
                Tuple<string, string, int> Can_Modify_CallTransferNum = new Tuple<string, string, int>("Can_Modify_CallTransferNum", "修改用户大号", 14);
                Tuple<string, string, int> Can_Delete_CallTransferNum = new Tuple<string, string, int>("Can_Delete_CallTransferNum", "删除用户大号", 14);

                Tuple<string, string, int> Can_Add_PaymentCard = new Tuple<string, string, int>("Can_Add_PaymentCard", "添加充值卡", 15);
                Tuple<string, string, int> Can_View_PaymentCard = new Tuple<string, string, int>("Can_View_PaymentCard", "查看充值卡", 15);
                Tuple<string, string, int> Can_Modify_PaymentCard = new Tuple<string, string, int>("Can_Modify_PaymentCard", "修改充值卡", 15);
                Tuple<string, string, int> Can_Delete_PaymentCard = new Tuple<string, string, int>("Can_Delete_PaymentCard", "删除充值卡", 15);

                Tuple<string, string, int> Can_Add_DeviceBracelet = new Tuple<string, string, int>("Can_Add_DeviceBracelet", "添加手环设备", 16);
                Tuple<string, string, int> Can_View_DeviceBracelet = new Tuple<string, string, int>("Can_View_DeviceBracelet", "查看手环设备", 16);
                Tuple<string, string, int> Can_Modify_DeviceBracelet = new Tuple<string, string, int>("Can_Modify_DeviceBracelet", "修改手环设备", 16);
                Tuple<string, string, int> Can_Delete_DeviceBracelet = new Tuple<string, string, int>("Can_Delete_DeviceBracelet", "删除手环设备", 16);

                Tuple<string, string, int> Can_Add_Feedback = new Tuple<string, string, int>("Can_Add_Feedback", "添加用户反馈", 17);
                Tuple<string, string, int> Can_View_Feedback = new Tuple<string, string, int>("Can_View_Feedback", "查看用户反馈", 17);
                Tuple<string, string, int> Can_Modify_Feedback = new Tuple<string, string, int>("Can_Modify_Feedback", "修改用户反馈", 17);
                Tuple<string, string, int> Can_Delete_Feedback = new Tuple<string, string, int>("Can_Delete_Feedback", "删除用户反馈", 17);

                Tuple<string, string, int> Can_Add_DeviceGoip = new Tuple<string, string, int>("Can_Add_DeviceGoip", "添加GOIP设备", 18);
                Tuple<string, string, int> Can_View_DeviceGoip = new Tuple<string, string, int>("Can_View_DeviceGoip", "查看GOIP设备", 18);
                Tuple<string, string, int> Can_Modify_DeviceGoip = new Tuple<string, string, int>("Can_Modify_DeviceGoip", "修改GOIP设备", 18);
                Tuple<string, string, int> Can_Delete_DeviceGoip = new Tuple<string, string, int>("Can_Delete_DeviceGoip", "删除GOIP设备", 18);

                Tuple<string, string, int> Can_Add_Sport = new Tuple<string, string, int>("Can_Add_Sport", "添加用户运动", 19);
                Tuple<string, string, int> Can_View_Sport = new Tuple<string, string, int>("Can_View_Sport", "查看用户运动", 19);
                Tuple<string, string, int> Can_Modify_Sport = new Tuple<string, string, int>("Can_Modify_Sport", "修改用户运动", 19);
                Tuple<string, string, int> Can_Delete_Sport = new Tuple<string, string, int>("Can_Delete_Sport", "删除用户运动", 19);

                Tuple<string, string, int> Can_Add_Banner = new Tuple<string, string, int>("Can_Add_Banner", "添加Banner", 20);
                Tuple<string, string, int> Can_View_Banner = new Tuple<string, string, int>("Can_View_Banner", "查看Banner", 20);
                Tuple<string, string, int> Can_Modify_Banner = new Tuple<string, string, int>("Can_Modify_Banner", "修改Banner", 20);
                Tuple<string, string, int> Can_Delete_Banner = new Tuple<string, string, int>("Can_Delete_Banner", "删除Banner", 20);

                Tuple<string, string, int> Can_Add_PageShow = new Tuple<string, string, int>("Can_Add_PageShow", "添加PageShow", 21);
                Tuple<string, string, int> Can_View_PageShow = new Tuple<string, string, int>("Can_View_PageShow", "查看PageShow", 21);
                Tuple<string, string, int> Can_Modify_PageShow = new Tuple<string, string, int>("Can_Modify_PageShow", "修改PageShow", 21);
                Tuple<string, string, int> Can_Delete_PageShow = new Tuple<string, string, int>("Can_Delete_PageShow", "删除PageShow", 21);

                Tuple<string, string, int> Can_Add_Config = new Tuple<string, string, int>("Can_Add_Config", "添加Config", 22);
                Tuple<string, string, int> Can_View_Config = new Tuple<string, string, int>("Can_View_Config", "查看Config", 22);
                Tuple<string, string, int> Can_Modify_Config = new Tuple<string, string, int>("Can_Modify_Config", "修改Config", 22);
                Tuple<string, string, int> Can_Delete_Config = new Tuple<string, string, int>("Can_Delete_Config", "删除Config", 22);

                Tuple<string, string, int> Can_Add_OrderByZC = new Tuple<string, string, int>("Can_Add_OrderByZC", "添加众筹订单", 23);
                Tuple<string, string, int> Can_View_OrderByZC = new Tuple<string, string, int>("Can_View_OrderByZC", "查看众筹订单", 23);
                Tuple<string, string, int> Can_Modify_OrderByZC = new Tuple<string, string, int>("Can_Modify_OrderByZC", "修改众筹订单", 23);
                Tuple<string, string, int> Can_Delete_OrderByZC = new Tuple<string, string, int>("Can_Delete_OrderByZC", "删除众筹订单", 23);

                Tuple<string, string, int> Can_Add_ZCSelectionNumber = new Tuple<string, string, int>("Can_Add_ZCSelectionNumber", "添加众筹选号", 24);
                Tuple<string, string, int> Can_View_ZCSelectionNumber = new Tuple<string, string, int>("Can_View_ZCSelectionNumber", "查看众筹选号", 24);
                Tuple<string, string, int> Can_Modify_ZCSelectionNumber = new Tuple<string, string, int>("Can_Modify_ZCSelectionNumber", "修改众筹选号", 24);
                Tuple<string, string, int> Can_Delete_ZCSelectionNumber = new Tuple<string, string, int>("Can_Delete_ZCSelectionNumber", "删除众筹选号", 24);

                Tuple<string, string, int> Can_Add_EjoinDev = new Tuple<string, string, int>("Can_Add_EjoinDev", "添加一正设备", 25);
                Tuple<string, string, int> Can_View_EjoinDev = new Tuple<string, string, int>("Can_View_EjoinDev", "查看一正设备", 25);
                Tuple<string, string, int> Can_Modify_EjoinDev = new Tuple<string, string, int>("Can_Modify_EjoinDev", "修改一正设备", 25);
                Tuple<string, string, int> Can_Delete_EjoinDev = new Tuple<string, string, int>("Can_Delete_EjoinDev", "删除一正设备", 25);

                Tuple<string, string, int> Can_Add_EjoinDevSlot = new Tuple<string, string, int>("Can_Add_EjoinDevSlot", "添加一正设备端口", 26);
                Tuple<string, string, int> Can_View_EjoinDevSlot = new Tuple<string, string, int>("Can_View_EjoinDevSlot", "查看一正设备端口", 26);
                Tuple<string, string, int> Can_Modify_EjoinDevSlot = new Tuple<string, string, int>("Can_Modify_EjoinDevSlot", "修改一正设备端口", 26);
                Tuple<string, string, int> Can_Delete_EjoinDevSlot = new Tuple<string, string, int>("Can_Delete_EjoinDevSlot", "删除一正设备端口", 26);

                returnList.Add(Can_Add_User);
                returnList.Add(Can_View_User);
                returnList.Add(Can_Modify_User);
                returnList.Add(Can_Delete_User);

                returnList.Add(Can_Add_ManageUser);
                returnList.Add(Can_View_ManageUser);
                returnList.Add(Can_Modify_ManageUser);
                returnList.Add(Can_Delete_ManageUser);

                returnList.Add(Can_Add_Role);
                returnList.Add(Can_View_Role);
                returnList.Add(Can_Modify_Role);
                returnList.Add(Can_Delete_Role);

                returnList.Add(Can_Add_Permission);
                returnList.Add(Can_View_Permission);
                returnList.Add(Can_Modify_Permission);
                returnList.Add(Can_Delete_Permission);

                returnList.Add(Can_Add_ManageUserRole);
                returnList.Add(Can_View_ManageUserRole);
                returnList.Add(Can_Modify_ManageUserRole);
                returnList.Add(Can_Delete_ManageUserRole);

                returnList.Add(Can_Add_RolePermission);
                returnList.Add(Can_View_RolePermission);
                returnList.Add(Can_Modify_RolePermission);
                returnList.Add(Can_Delete_RolePermission);

                returnList.Add(Can_Add_UserBill);
                returnList.Add(Can_View_UserBill);
                returnList.Add(Can_Modify_UserBill);
                returnList.Add(Can_Delete_UserBill);

                returnList.Add(Can_Add_Order);
                returnList.Add(Can_View_Order);
                returnList.Add(Can_Modify_Order);
                returnList.Add(Can_Delete_Order);

                returnList.Add(Can_Add_Package);
                returnList.Add(Can_View_Package);
                returnList.Add(Can_Modify_Package);
                returnList.Add(Can_Delete_Package);

                returnList.Add(Can_Add_Payment);
                returnList.Add(Can_View_Payment);
                returnList.Add(Can_Modify_Payment);
                returnList.Add(Can_Delete_Payment);

                returnList.Add(Can_Add_SMS);
                returnList.Add(Can_View_SMS);
                returnList.Add(Can_Modify_SMS);
                returnList.Add(Can_Delete_SMS);

                returnList.Add(Can_Add_Country);
                returnList.Add(Can_View_Country);
                returnList.Add(Can_Modify_Country);
                returnList.Add(Can_Delete_Country);

                returnList.Add(Can_Add_SpeakRecord);
                returnList.Add(Can_View_SpeakRecord);
                returnList.Add(Can_Modify_SpeakRecord);
                returnList.Add(Can_Delete_SpeakRecord);

                returnList.Add(Can_Add_CallTransferNum);
                returnList.Add(Can_View_CallTransferNum);
                returnList.Add(Can_Modify_CallTransferNum);
                returnList.Add(Can_Delete_CallTransferNum);

                returnList.Add(Can_Add_PaymentCard);
                returnList.Add(Can_View_PaymentCard);
                returnList.Add(Can_Modify_PaymentCard);
                returnList.Add(Can_Delete_PaymentCard);

                returnList.Add(Can_Add_DeviceBracelet);
                returnList.Add(Can_View_DeviceBracelet);
                returnList.Add(Can_Modify_DeviceBracelet);
                returnList.Add(Can_Delete_DeviceBracelet);

                returnList.Add(Can_Add_Feedback);
                returnList.Add(Can_View_Feedback);
                returnList.Add(Can_Modify_Feedback);
                returnList.Add(Can_Delete_Feedback);

                returnList.Add(Can_Add_DeviceGoip);
                returnList.Add(Can_View_DeviceGoip);
                returnList.Add(Can_Modify_DeviceGoip);
                returnList.Add(Can_Delete_DeviceGoip);

                returnList.Add(Can_Add_Sport);
                returnList.Add(Can_View_Sport);
                returnList.Add(Can_Modify_Sport);
                returnList.Add(Can_Delete_Sport);

                returnList.Add(Can_Add_Banner);
                returnList.Add(Can_View_Banner);
                returnList.Add(Can_Modify_Banner);
                returnList.Add(Can_Delete_Banner);

                returnList.Add(Can_Add_PageShow);
                returnList.Add(Can_View_PageShow);
                returnList.Add(Can_Modify_PageShow);
                returnList.Add(Can_Delete_PageShow);

                returnList.Add(Can_Add_Config);
                returnList.Add(Can_View_Config);
                returnList.Add(Can_Modify_Config);
                returnList.Add(Can_Delete_Config);

                returnList.Add(Can_Add_OrderByZC);
                returnList.Add(Can_View_OrderByZC);
                returnList.Add(Can_Modify_OrderByZC);
                returnList.Add(Can_Delete_OrderByZC);

                returnList.Add(Can_Add_ZCSelectionNumber);
                returnList.Add(Can_View_ZCSelectionNumber);
                returnList.Add(Can_Modify_ZCSelectionNumber);
                returnList.Add(Can_Delete_ZCSelectionNumber);

                returnList.Add(Can_Add_EjoinDev);
                returnList.Add(Can_View_EjoinDev);
                returnList.Add(Can_Modify_EjoinDev);
                returnList.Add(Can_Delete_EjoinDev);

                returnList.Add(Can_Add_EjoinDevSlot);
                returnList.Add(Can_View_EjoinDevSlot);
                returnList.Add(Can_Modify_EjoinDevSlot);
                returnList.Add(Can_Delete_EjoinDevSlot);
                #endregion

                return returnList;
            }
        }
    }
}
