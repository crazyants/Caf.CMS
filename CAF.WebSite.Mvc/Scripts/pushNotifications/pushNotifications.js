

var PushNotificationService = function () {

    var handlePushNotification = function () {
        var signalRServerName = window.location.host;
        var clientPushHubProxy = signalRHubProxy('', 'clientPushHub', { logging: true });
        clientPushHubProxy.on('notification', function (data) {
            //var notifyMenu = mainMenuService.findByPath('pushNotifications');
            //var notificationTemplate = eventTemplateResolver.resolve(data, 'menu');
            ////broadcast event
            //$rootScope.$broadcast("new-notification-event", data);

            //var menuItem = {
            //    parent: notifyMenu,
            //    path: 'pushNotifications/notifications',
            //    icon: 'fa fa-bell-o',
            //    title: data.title,
            //    priority: 2,
            //    permission: '',
            //    children: [],
            //    action: notificationTemplate.action,
            //    template: notificationTemplate.template,
            //    notify: data
            //};

            //var alreadyExitstItem = _.find(notifyMenu.children, function (x) { return x.id == menuItem.id; });
            //if (alreadyExitstItem) {
            //    angular.copy(menuItem, alreadyExitstItem);
            //}
            //else {
            //    notifyMenu.children.push(menuItem);
            //    notifyMenu.newCount++;
            //}
            //notifyMenu.incremented = true;
        });
    }


    return {
        init: function () {
            handlePushNotification();

        }

    };

}();