
    function AddComment(_deckId)
    {
        console.log('adding new comment');
        //Serialize the form datas.
        var form = $('#form_comment-' + _deckId)[0];
        var valdata = $(form).serialize();
        var dataArr = $(form).serializeArray();

        console.log(valdata);
        console.log(dataArr);

        var base = window.location.origin;
        var payload = JSON.stringify({
            DeckId: dataArr[0].value,
            Content: dataArr[1].value
        })

        // send post request
        $.ajax({
            url: base+'/api/Comments/AddComment',
            method: "POST", 
            dataType: 'json',
            contentType: "application/json",
            data: payload,
            success: function (result) {
        // fetch respond data
                var commentelement = GetCommentTemplate(result.value);

                var parentContainer = $("#commentsSection-"+_deckId); 

                parentContainer[0].insertAdjacentHTML('beforeend', commentelement) 
            },
            error: function (err) {
                console.log(err.Message);
            }
        });
    }

    function GetCommentTemplate(commentObject) {
    var date = new Date(commentObject.created);
    var dateString = minTwoDigits(date.getDate()) + '.' +minTwoDigits((date.getMonth() + 1)) + "." +date.getFullYear() + ' ' + minTwoDigits(date.getHours()) + ":" +minTwoDigits(date.getMinutes()) + ":" + minTwoDigits(date.getSeconds());

        var htmldata =
            '<div id="mainComment-' + commentObject.id + '" class="card border-0">' +
                '<div class=" card-body p-0">'+
                    '<div class="d-flex">'+
                        '<div class="pt-2 pr-1">'+
                            '<img alt="Image" src="'+commentObject.author.avatarImage+'" class="avatar avatar-sm d-block">'+
                        '</div>'+
                        '<div class="comment">'+
                            ' <div style="font-weight:bold;"> '+commentObject.author.userName+' </div>'+
                            ' <div>  '+commentObject.content+'  </div>'+
                        ' </div>'+
                    '</div>'+
                    '<a href="*" class="reply-btn"> Reply </a>' +
                        '<small> ' + dateString +' </small><br>' +
                    '</div>' +
                '</div>';

    return htmldata;

    function minTwoDigits(n) {
        return (n < 10 ? '0' : '') + n;
    }
}
   
function ShowReplies(_replyIds) {
    console.log("show replies");
    _replyIds.forEach( (_id) => {
        var el = $("#replyComment-" + _id);
        el[0].removeAttribute("hidden");
    })
}