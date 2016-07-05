
;
(function ($, window, document, undefined) {

$(function () {

    $(".article-review-item").on("click", ".vote", function (e) {
        var el = $(this);
        var reviewId = el.parent().data("review-id");
        var href = el.parent().data("href");
        var isNo = el.hasClass("vote-no");

        setProductReviewHelpfulness(reviewId, isNo ? 'false' : 'true');

        function setProductReviewHelpfulness(reviewId, wasHelpful) {
            $.ajax({
                cache: false,
                type: "POST",
                url: href,
                data: { "articleReviewId": reviewId, "washelpful": wasHelpful },
                success: function (data) {
                   
                    if (data.Result) {
                        toastr[data.Success ? "success" : "error"](data.Result, '');
                       
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Failed to vote. Please refresh the page and try one more time.');
                }
            });
        }
        return false;
    });

});

})( jQuery, this, document );

