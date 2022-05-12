﻿function ToggleComments(btn, _deckId) {
    var parentobject = $("#Comments-" + _deckId)[0];
    var x = $(btn).attr('toggled');
    if (x === "false") {
        // GET call for comments data
        var base = window.location.origin;
        var url = base + '/Comment/LoadCommentByDeck';
        $.get(url,
            {
                deckId: _deckId
            },
            function (data) {
                // change state (opened)
                console.log("toggle comments");
                $(btn).attr("toggled", true);
                // populate content
                parentobject.innerHTML = data;

                // show, adjust size by adding classes
                parentobject.classList.add('col-12', 'm-1', 'mt-n2', 'mb-3');
            }
        );
    }
    else {
        // change state (closed)
        console.log("hide comments");
        $(btn).attr("toggled", false);

        // clear content
        parentobject.innerHTML = "";

        // hide remove size classes
        parentobject.classList.remove('col-12', 'm-1', 'mt-n2', 'mb-3');
    }

}

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
            var commentelement = GetCommentTemplate('replyComment', result.value);

            if (dataArr[3].value == '1') {
                var parentContainer = $("#mainComment-" + dataArr[0].value)
                parentContainer[0].insertAdjacentHTML('beforeend', commentelement);
            }
            else {
                form.insertAdjacentHTML('beforebegin', commentelement);
            }

            // hide input button
            form.hidden = true;
        },
        error: function (err) {
            console.log(err.Message);
        }
    });
}

    function GetCommentTemplate(template,commentObject) {
        var date = new Date(commentObject.created);
        var formattedDate = minTwoDigits(date.getDate()) + '.' +minTwoDigits((date.getMonth() + 1)) + "." +date.getFullYear() + ' ' + minTwoDigits(date.getHours()) + ":" +minTwoDigits(date.getMinutes()) + ":" + minTwoDigits(date.getSeconds());
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
                '</div>';

            if (commentObject.depth < 3) {
                htmldata+='<a href="*" style="pointer-events: none; cursor: default ; opacity: 0.6; " class="reply-btn"> Reply </a>';
            }

                htmldata+='<small> ' + formattedDate + ' </small><br>' +
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
                '<a href="*" style="pointer-events: none; cursor: default ; opacity: 0.6; " class="reply-btn"> Reply </a>' +
                '<small> ' + formattedDate + ' </small><br>' +
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

        // Check what action take first element, than apply this to rest
        //  when user type new comment, and then open hidden comment, 
        //  his comment would be set as hidden,
 
        for (let item of parent) {
            if (item.id.includes('replyComment-')) {
                if (item.hasAttribute('hidden')) {
                    revealAll(parent);
                    return;
                }
                else {
                    hideall(parent);
                    return;
                }}
            }
        function hideall(items) {
            for (let item of items) {
                if (item.id.includes('replyComment-')) {
                    item.hidden = true;
                }
            }
        }
        function revealAll(items) {
            for (let item of items) {
                if (item.id.includes('replyComment-')) {
                    if (item.hasAttribute('hidden')) {
                        item.removeAttribute('hidden');
                    }
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



$(function () {
    $(window).keydown(function (e) {
        if (e.keyCode == 13) {
            console.log('prevent enter to submit-redirect form');
            e.preventDefault();
            return false;
        }
    });
})