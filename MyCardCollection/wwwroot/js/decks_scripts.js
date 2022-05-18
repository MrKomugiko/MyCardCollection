// Get from server partial view of deck dontent overview, and paste it in modal
function LoadDeckOverview(_deckId)
{
    $.ajax({
        url: '/Decks/Overview',
        datatype: 'html',
        method: 'GET',
        data: {
            deckId: _deckId
        },
        success: function (res) {
            $('#DeckOverview_Partial').html('').html(res);
        },
        error: function (err) {
            console.log(err);
        }
    })
}

// hide normal and reveal edit 'layout' on deck when Edit button
function ShowEditLayout(btn, deckId)
{
    // hide 'normal' deck tamplate
    var deckNormal = btn.closest('#deck-'+deckId);
    deckNormal.style.display = "none";
    // load 'edit' deck template
    var deckEdit = deckNormal.nextElementSibling;
    deckEdit.style.display = "block";
}

// just hide modal on editform cancel
function Cancel(btn, deckId) {
    var editDeck = btn.closest('#editDeck-' + deckId);
    editDeck.style.display = 'none';
    editDeck.previousElementSibling.style = "block";
}

// fetch data from form (while editing deck) send POST request to update deck object
//      then replace data with updated one (local, no need page reload to refresh)
function postedit(_deckId) {
    //Serialize the form datas.   
    var valdata = $('#editform-' + _deckId).serialize();
    var dataArr = $('#editform-' + _deckId).serializeArray();

    $.ajax({
        url: '/Decks/EditDeck',
        method: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: valdata,
        success: function (result) {
            var editDeck = document.getElementById('editDeck-' + _deckId);
            var normalDeckView = document.getElementById("deck-" + _deckId)

            // replace data on main page
            var ID = dataArr[0].value;
            document.getElementById("deckBackgroundImage-" + _deckId).src = dataArr[1].value;
            document.getElementById("deckName-" + _deckId).textContent = dataArr[2].value;
            document.getElementById("deckDescription-" + _deckId).textContent = dataArr[3].value;

            // close window
            editDeck.style.display = 'none';
            normalDeckView.style = "block";

        },
        error: function (err) {
            console.log("error edition failed");
        }
    });
}

// open modal when press image while editing, fetch GET list of available backgrounds
//      then insert into modal list of images
function openBackgroundSelectionWindow(_deckId) {
    $('#deckBackgroundSelectionModal').modal('show');

    var content = document.getElementById('deckBackgroundSelectionModal').getElementsByClassName('content')[0];
    content.innerHTML = "";

    $.get("/Decks/GetDeckBackgrounds", { deckId: _deckId },
        function (data, status) {
            console.log("Data: " + data + "\nStatus: " + status);

            data.value.forEach(url => {
                content.insertAdjacentHTML('beforeend',
                    '<div class="col-6 col-sm-4 col-lg-3 col-xl-2 pb-3">' +
                    '<a type="button" onclick=\'lockSelection(this,\"' + url + '\",' + _deckId + ')\'>' +
                    '<img class="img-fluid" style="border-radius:10px;" data-id="' + _deckId + '"src="' + url + '" />' +
                    '</a>' +
                    '</div>'
                )
            })
        }
    );
}

// pass selected image url to hidden field form (under modal), and refresh form ui with selected bg
function lockSelection(img, _newSrc, _deckId) {
    // hide modal after select image
    $('#deckBackgroundSelectionModal').modal('hide');
    // assing to hidden img input
    $('#editInputBackgroundImage-' + _deckId)[0].value = _newSrc;
    // replace in edit view
    $('#editDeckBackgroundImage-' + _deckId)[0].src = _newSrc;
}

function Like(_btn, _deckId, _userId) {
    var base = window.location.origin;
    var payload = JSON.stringify({
        deckId: _deckId,
        userId: _userId,
    });

    var url = base + '/api/Like/Deck';
    $.ajax({
        type: "POST",
        url: url,
        content: "json",
        contentType: "application/json",
            data: payload,
            success: function (result) {
                if (result > 0) {
                    // deck liked succesfully, get return back its id

                    _btn.classList.remove("btn-outline-primary");
                    _btn.classList.add("btn-danger");

                    var currentValue = parseInt(_btn.children[0].nextSibling.textContent);
                    _btn.children[0].nextSibling.textContent = (currentValue + 1); 

                    _btn.onclick = function () {
                        Disslike(_btn, _deckId, _userId);
                    }

                    console.log('created new like with Id: #'+result+'. Liked - new value = ' + (currentValue + 1));
                }
            },
            error: function (message) {
                console.log(message);
            }
        });
    }

function Disslike(_btn, _deckId, _userId) {

    var base = window.location.origin;
    var payload = JSON.stringify({
        deckId: _deckId,
        userId: _userId,
    });

    var url = base + '/api/Like/Deck';
    $.ajax({
        type: "DELETE",
        url: url,
        content: "json",
        contentType: "application/json",
        data: payload,
        success: function (result) {
          
            _btn.classList.remove("btn-danger");
            _btn.classList.add("btn-outline-primary");

            var currentValue = parseInt(_btn.children[0].nextSibling.textContent);
            _btn.children[0].nextSibling.textContent = (currentValue - 1); 

            _btn.onclick = function () {
                Like(_btn, _deckId, _userId);
            }
            console.log('dissliked - new value = '+(currentValue - 1));
            
        },
        error: function (message) {
            console.log(message);
        }
    });
}
