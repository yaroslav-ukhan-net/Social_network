window.onload = function () {
    var page = 0;
    var _inCallback = false;
    function loadItems() {
        if (page > -1 && !_inCallback) {
            _inCallback = true;
            page++;
            id = 1;

            $.ajax({
                type: 'GET',
                url: '/Chat/ChatWithUser/',
                data: { id: id, pageMessage: page },
                success: function (data, textstatus) {
                    if (data != '') {
                        $("#scrolList").append(data);
                    }
                    else {
                        page = -1;
                    }
                    _inCallback = false;
                }
            });
        }
    }
    window.addEventListener('scroll', function () {
        if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight) {

            loadItems();
        }
    });
}
