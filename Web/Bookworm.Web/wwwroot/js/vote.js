$("[data-vote]").each(function (el) {
    $(this).on("click", function () {
        var value = $(this).attr("data-vote");
        var bookId = '@Model.Id';
        var antiForgeryToken = $('#antiForgeryForm input[name=__RequestVerificationToken]').val();
        var data = { bookId: bookId, value: value };
        $.ajax({
            type: "POST",
            url: "/api/Vote",
            data: JSON.stringify(data),
            headers: {
                'X-CSRF-TOKEN': antiForgeryToken
            },
            contentType: 'application/json',
            success: function (data) {
                $('#averageVoteValue').html(data.averageVote.toFixed(1));
                $('#votesCount').html(data.votesCount);
                var userVote = data.userVote;
                if (userVote == 1) {
                    $('#1-star-rating').prop('checked', true);
                    $('#2-star-rating').prop('checked', false);
                    $('#3-star-rating').prop('checked', false);
                    $('#4-star-rating').prop('checked', false);
                    $('#5-star-rating').prop('checked', false);
                } else if (userVote == 2) {
                    $('#1-star-rating').prop('checked', true);
                    $('#2-star-rating').prop('checked', true);
                    $('#3-star-rating').prop('checked', false);
                    $('#4-star-rating').prop('checked', false);
                    $('#5-star-rating').prop('checked', false);
                } else if (userVote == 3) {
                    $('#1-star-rating').prop('checked', true);
                    $('#2-star-rating').prop('checked', true);
                    $('#3-star-rating').prop('checked', true);
                    $('#4-star-rating').prop('checked', false);
                    $('#5-star-rating').prop('checked', false);
                } else if (userVote == 4) {
                    $('#1-star-rating').prop('checked', true);
                    $('#2-star-rating').prop('checked', true);
                    $('#3-star-rating').prop('checked', true);
                    $('#4-star-rating').prop('checked', true);
                    $('#5-star-rating').prop('checked', false);
                } else if (userVote == 5) {
                    $('#1-star-rating').prop('checked', true);
                    $('#2-star-rating').prop('checked', true);
                    $('#3-star-rating').prop('checked', true);
                    $('#4-star-rating').prop('checked', true);
                    $('#5-star-rating').prop('checked', true);
                } else {

                }
            }
        });
    })
});