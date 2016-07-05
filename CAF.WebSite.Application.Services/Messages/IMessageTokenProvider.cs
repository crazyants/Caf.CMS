
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Users;
using System.Collections.Generic;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
//using CAF.Infrastructure.Core.Domain.Shop.Orders;
//using CAF.Infrastructure.Core.Domain.Shop.Shipping;
//using CAF.Infrastructure.Core.Domain.Shop.Catalog;

namespace CAF.WebSite.Application.Services.Messages
{
    public partial interface IMessageTokenProvider
    {
        void AddSiteTokens(IList<Token> tokens, Site site);

        //void AddOrderTokens(IList<Token> tokens, Order order, int languageId);

        //void AddShipmentTokens(IList<Token> tokens, Shipment shipment, int languageId);

        //void AddOrderNoteTokens(IList<Token> tokens, OrderNote orderNote);

        //void AddRecurringPaymentTokens(IList<Token> tokens, RecurringPayment recurringPayment);

        //void AddReturnRequestTokens(IList<Token> tokens, ReturnRequest returnRequest, OrderItem orderItem);

        //void AddGiftCardTokens(IList<Token> tokens, GiftCard giftCard);

        void AddUserTokens(IList<Token> tokens, User user);

        void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription);

        //void AddProductReviewTokens(IList<Token> tokens, ProductReview productReview);

       // void AddBlogCommentTokens(IList<Token> tokens, BlogComment blogComment);

        //void AddNewsCommentTokens(IList<Token> tokens, NewsComment newsComment);

        //void AddProductTokens(IList<Token> tokens, Product product, int languageId);

        void AddForumTokens(IList<Token> tokens, Forum forum);

        void AddForumTopicTokens(IList<Token> tokens, ForumTopic forumTopic,
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null);

        void AddForumPostTokens(IList<Token> tokens, ForumPost forumPost);

        void AddPrivateMessageTokens(IList<Token> tokens, PrivateMessage privateMessage);

        //void AddBackInStockTokens(IList<Token> tokens, BackInStockSubscription subscription);

        string[] GetListOfCampaignAllowedTokens();

        string[] GetListOfAllowedTokens();

        //codehint: sm-add begin
        void AddBankConnectionTokens(IList<Token> tokens);

        void AddCompanyTokens(IList<Token> tokens);

        void AddContactDataTokens(IList<Token> tokens);
        //codehint: sm-add end

    }
}
