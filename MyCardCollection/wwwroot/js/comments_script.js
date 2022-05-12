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
        var payload = JSON.stringify(
            {
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
                var commentelement = GetCommentTemplate('mainComment',result.value);

                var parentContainer = $("#commentsSection-"+_deckId); 

                parentContainer[0].insertAdjacentHTML('beforeend', commentelement) 
            },
            error: function (err) {
                console.log(err.Message);
            }
        });
}

function AddReply(formId) {
    console.log('adding new reply');
    //Serialize the form datas.
    var form = $(formId)[0];
    var valdata = $(form).serialize();
    var dataArr = $(form).serializeArray();

    console.log(valdata);
    console.log(dataArr);

    var replytoid = -1;
    if (dataArr[2].value !== "") {
        replytoid = dataArr[2].value;
    }

    var base = window.location.origin;
    var payload = JSON.stringify(
        {
            CommentId: dataArr[0].value,
            ReplyTo: replytoid,
            Depth: dataArr[3].value,
            Content: dataArr[1].value,
        })
    console.log(payload);

    console.log(JSON.stringify(payload));
    // send post request
    $.ajax({
        url: base + '/api/Comments/AddReply',
        method: "POST",
        dataType: 'json',
        contentType: "application/json",
        data: payload,
        success: function (result) {
            // fetch respond data

            var parentContainer;
            if (dataArr[3].value == '1') {
                parentContainer = $("#mainComment-" + dataArr[0].value)
            }
            else {
                parentContainer = $("#replyComment-" + dataArr[2].value);
                parentContainer = parentContainer[0];
                parentContainer = parentContainer.children;
            }

            var commentelement = GetCommentTemplate('replyComment', result.value);


            parentContainer[0].insertAdjacentHTML('beforeend', commentelement)
        },
        error: function (err) {
            console.log(err.Message);
        }
    });
}

    function GetCommentTemplate(template,commentObject) {
        var date = new Date(commentObject.created);
        var dateString = minTwoDigits(date.getDate()) + '.' +minTwoDigits((date.getMonth() + 1)) + "." +date.getFullYear() + ' ' + minTwoDigits(date.getHours()) + ":" +minTwoDigits(date.getMinutes()) + ":" + minTwoDigits(date.getSeconds());
        var htmldata = "";
        if (template.includes("reply")) {
            htmldata =
                '<div id="' + template + '-' + commentObject.id + '" class="card border-0 m-0">' +
                '<div class="card-body pt-1 pb-1 pr-0 pl-1 pl-md-5">' +
                '<div class="d-flex">' +
                '<div class="pt-2 pr-1">' +
                '<img alt="Image" src="' + commentObject.author.avatarImage + '" class="avatar avatar-sm d-block">' +
                '</div>' +
                '<div class="comment">' +
                ' <div style="font-weight:bold;"> ' + commentObject.author.userName + ' </div>' +
                ' <div>  ' + commentObject.content + '  </div>' +
                ' </div>' +
                '</div>' +
                '<a href="*" style="pointer-events: none; cursor: default ; opacity: 0.6; " class="reply-btn"> Reply </a>' +
                '<small> ' + dateString + ' </small><br>' +
                '</div>' +
                '</div>';
        }

        else {
             htmldata =
                '<div id="' + template + '-' + commentObject.id + '" class="card border-0">' +
                '<div class=" card-body p-0">' +
                '<div class="d-flex">' +
                '<div class="pt-2 pr-1">' +
                '<img alt="Image" src="' + commentObject.author.avatarImage + '" class="avatar avatar-sm d-block">' +
                '</div>' +
                '<div class="comment">' +
                ' <div style="font-weight:bold;"> ' + commentObject.author.userName + ' </div>' +
                ' <div>  ' + commentObject.content + '  </div>' +
                ' </div>' +
                '</div>' +
                '<a href="*" class="reply-btn"> Reply </a>' +
                '<small> ' + dateString + ' </small><br>' +
                '</div>' +
                '</div>';
        }
         

        return htmldata;

        function minTwoDigits(n) {
            return (n < 10 ? '0' : '') + n;
        }
    }
   
    function ToggleRepiles(containerId) {
        console.log("show replies");

        var parent = $(containerId)[0].children[0].children;
        var p = parent[0];

        for (let item of parent) {
            if (item.id.includes('replyComment-')) {

                console.log(item.id);

                if (item.hasAttribute('hidden')) {
                    item.removeAttribute('hidden');
                }
                else {
                    item.hidden = true;
                }
            }
        }
    }

    function ToggleReplyForm(_replyToId) {
        console.log("show form input");

        var parentForm = $(_replyToId)[0];

        if (parentForm.hasAttribute('hidden')) {
            parentForm.removeAttribute('hidden');
        }
        else {
            parentForm.hidden = true;
        }
    }

