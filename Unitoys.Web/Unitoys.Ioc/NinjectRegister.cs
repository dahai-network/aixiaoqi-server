using Ninject;
using Ninject.Web.Mvc.FilterBindingSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Unitoys.IServices;
using Unitoys.Services;
using Unitoys.Core;

namespace Unitoys.Ioc
{
    public class NinjectRegister
    {
        private static readonly IKernel Kernel;
        static NinjectRegister()
        {
            Kernel = new StandardKernel();
            AddBindings();
            AddFilterBindings();
        }

        public static void RegisterFovMvc()
        {
            DependencyResolver.SetResolver(new NinjectDependencyResolverForMvc(Kernel));
        }

        public static void RegisterFovWebApi(HttpConfiguration config)
        {
            config.DependencyResolver = new NinjectDependencyResolverForWebApi(Kernel);
        }

        private static void AddBindings()
        {
            Kernel.Bind<IUserService>().To<UserService>();
            Kernel.Bind<IManageUserService>().To<ManageUserService>();
            Kernel.Bind<ISMSService>().To<SMSService>();
            Kernel.Bind<IPackageService>().To<PackageService>();
            Kernel.Bind<ICountryService>().To<CountryService>();
            Kernel.Bind<IUserBillService>().To<UserBillService>();
            Kernel.Bind<IPaymentService>().To<PaymentService>();
            Kernel.Bind<IUserLoginRecordService>().To<UserLoginInfoService>();
            Kernel.Bind<IDeviceGoipService>().To<DeviceGoipService>();
            Kernel.Bind<IDeviceBraceletService>().To<DeviceBraceletService>();
            Kernel.Bind<IOrderService>().To<OrderService>();
            Kernel.Bind<IOrderUsageService>().To<OrderUsageService>();
            Kernel.Bind<ISpeakRecordService>().To<SpeakRecordService>();
            Kernel.Bind<IMessageService>().To<MessageService>();
            Kernel.Bind<IMessagePhotoService>().To<MessagePhotoService>();
            Kernel.Bind<IMessageCommentService>().To<MessageCommentService>();
            Kernel.Bind<IMessageLikeService>().To<MessageLikeService>();
            Kernel.Bind<IFeedbackService>().To<FeedbackService>();
            Kernel.Bind<ISMSConfirmationService>().To<SMSConfirmationService>();
            Kernel.Bind<IRoleService>().To<RoleService>();
            Kernel.Bind<IPermissionService>().To<PermissionService>();
            Kernel.Bind<IRolePermissionService>().To<RolePermissionService>();
            Kernel.Bind<IManageUsersRoleService>().To<ManageUsersRoleService>();
            Kernel.Bind<IPhoneCallbackService>().To<PhoneCallbackService>();
            Kernel.Bind<ICallTransferNumService>().To<CallTransferNumService>();
            Kernel.Bind<IPaymentCardService>().To<PaymentCardService>();
            Kernel.Bind<IUserShapeService>().To<UserShapeService>();
            Kernel.Bind<ISportService>().To<SportService>();
            Kernel.Bind<ISportTimePeriodService>().To<SportTimePeriodService>();
            Kernel.Bind<IUserWxService>().To<UserWxService>();
            Kernel.Bind<IBannerService>().To<BannerService>();
            Kernel.Bind<IAlarmClockService>().To<AlarmClockService>();
            Kernel.Bind<IUsersConfigService>().To<UsersConfigService>();
            Kernel.Bind<IPageShowService>().To<PageShowService>();
            Kernel.Bind<IOrderByZCService>().To<OrderByZCService>();
            Kernel.Bind<IOrderByZCSelectionNumberService>().To<OrderByZCSelectionNumberService>();
            Kernel.Bind<IZCSelectionNumberService>().To<ZCSelectionNumberService>();
            Kernel.Bind<IOrderByZCConfirmationService>().To<OrderByZCConfirmationService>();
            Kernel.Bind<IEjoinDevService>().To<EjoinDevService>();
            Kernel.Bind<IEjoinDevSlotService>().To<EjoinDevSlotService>();
            Kernel.Bind<IGiftCardService>().To<GiftCardService>();
            Kernel.Bind<IOperationRecordService>().To<OperationRecordService>();
        }

        private static void AddFilterBindings()
        {
            Kernel.BindFilter<UnitoysAuthorizeAttribute>(FilterScope.Controller, 0);
        }

        /// <summary>
        /// Creates the kernel.
        /// </summary>
        /// <returns>the newly created kernel.</returns>
        public static IKernel CreateKernel()
        {
            return Kernel;
        }

        /// <summary>
        /// 获取Kernel对应服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetKernelService(Type t)
        {
            return Kernel.Get(t);
        }
    }
}
