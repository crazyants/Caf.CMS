
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Domain.Security;
//using CAF.Infrastructure.Core.Domain.Shop.Catalog;
//using CAF.Infrastructure.Core.Domain.Shop.Orders;
//using CAF.Infrastructure.Core.Domain.Shop.Shipping;
using CAF.Infrastructure.Core.Domain.Users;
using System;


namespace CAF.WebSite.Application.Services.Messages
{
    public partial interface IWorkflowMessageService
    {
        #region User workflow

        /// <summary>
        /// Sends 'New user' notification message to a store owner
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendUserRegisteredNotificationMessage(User user, int languageId);

        /// <summary>
        /// Sends a welcome message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendUserWelcomeMessage(User user, int languageId);

        /// <summary>
        /// Sends an email validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendUserEmailValidationMessage(User user, int languageId);

        /// <summary>
        /// Sends password recovery message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendUserPasswordRecoveryMessage(User user, int languageId);

        #endregion

        #region Order workflow

        ///// <summary>
        ///// Sends an order placed notification to a store owner
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //int SendOrderPlacedSiteOwnerNotification(Order order, int languageId);

        ///// <summary>
        ///// Sends an order placed notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //int SendOrderPlacedUserNotification(Order order, int languageId);

        ///// <summary>
        ///// Sends a shipment sent notification to a user
        ///// </summary>
        ///// <param name="shipment">Shipment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //int SendShipmentSentUserNotification(Shipment shipment, int languageId);

        ///// <summary>
        ///// Sends a shipment delivered notification to a user
        ///// </summary>
        ///// <param name="shipment">Shipment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //int SendShipmentDeliveredUserNotification(Shipment shipment, int languageId);

        ///// <summary>
        ///// Sends an order completed notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //int SendOrderCompletedUserNotification(Order order, int languageId);

        ///// <summary>
        ///// Sends an order cancelled notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //int SendOrderCancelledUserNotification(Order order, int languageId);

        ///// <summary>
        ///// Sends a new order note added notification to a user
        ///// </summary>
        ///// <param name="orderNote">Order note</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //int SendNewOrderNoteAddedUserNotification(OrderNote orderNote, int languageId);

        ///// <summary>
        ///// Sends a "Recurring payment cancelled" notification to a store owner
        ///// </summary>
        ///// <param name="recurringPayment">Recurring payment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //int SendRecurringPaymentCancelledSiteOwnerNotification(RecurringPayment recurringPayment, int languageId);

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription,
            int languageId);

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription,
            int languageId);

        #endregion

        #region Send a message to a friend

        /// <summary>
        /// Sends "email a friend" message
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="product">Product instance</param>
        /// <param name="userEmail">User's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        //int SendProductEmailAFriendMessage(User user, int languageId,
        //    Product product, string userEmail, string friendsEmail, string personalMessage);

        //int SendProductQuestionMessage(User user, int languageId, Product product,
        //    string senderEmail, string senderName, string senderPhone, string question);

        /// <summary>
        /// Sends wishlist "email a friend" message
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="userEmail">User's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        int SendWishlistEmailAFriendMessage(User user, int languageId,
             string userEmail, string friendsEmail, string personalMessage);

        #endregion

        #region Return requests

        /// <summary>
        /// Sends 'New Return Request' message to a store owner
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        // int SendNewReturnRequestSiteOwnerNotification(ReturnRequest returnRequest, OrderItem orderItem, int languageId);


        /// <summary>
        /// Sends 'Return Request status changed' message to a user
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        // int SendReturnRequestStatusChangedUserNotification(ReturnRequest returnRequest, OrderItem orderItem, int languageId);

        #endregion

        #region Forum Notifications

        /// <summary>
        /// Sends a forum subscription message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendNewForumTopicMessage(User user,
            ForumTopic forumTopic, Forum forum, int languageId);

        /// <summary>
        /// Sends a forum subscription message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendNewForumPostMessage(User user,
            ForumPost forumPost, ForumTopic forumTopic,
            Forum forum, int friendlyForumTopicPageIndex,
            int languageId);

        /// <summary>
        /// Sends a private message notification
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendPrivateMessageNotification(User user, PrivateMessage privateMessage, int languageId);

        #endregion

        #region Misc

        /// <summary>
        /// Sends a product review notification message to a store owner
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        //int SendProductReviewNotificationMessage(ProductReview productReview,
        //    int languageId);

        /// <summary>
        /// Sends a gift card notification
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        // int SendGiftCardNotification(GiftCard giftCard, int languageId);


        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        // int SendQuantityBelowSiteOwnerNotification(Product product, int languageId);


        /// <summary>
        /// Sends a "new VAT sumitted" notification to a store owner
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendNewVatSubmittedSiteOwnerNotification(User user,
            string vatName, string vatAddress, int languageId);

        /// <summary>
        /// Sends a blog comment notification message to a store owner
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        //int SendBlogCommentNotificationMessage(BlogComment blogComment, int languageId);

        /// <summary>
        /// Sends a news comment notification message to a store owner
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        // int SendNewsCommentNotificationMessage(NewsComment newsComment, int languageId);

        /// <summary>
        /// Sends a 'Back in stock' notification message to a user
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        //int SendBackInStockNotification(BackInStockSubscription subscription, int languageId);

        /// <summary>
        /// Sends a generic message
        /// </summary>
        /// <param name="messageTemplateName">The name of the message template</param>
        /// <param name="cfg">Configurator action for the message</param>
        /// <returns>Queued email identifier</returns>
        int SendGenericMessage(string messageTemplateName, Action<GenericMessageContext> cfg);

        #endregion
    }
}
