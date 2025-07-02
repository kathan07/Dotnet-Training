var Common = {

    init: function () {
        $("#divFilters").hide();
        $("#divSorting").hide();
        $(".overlay").css('display', 'none');
        $('.select2').select2();
    },

    /* this is used to show filter on listing screen */
    showFilter: function (element, id) {
        $(id).toggle();
        $(id).css({
            'z-index': '2',  // Increase z-index dynamically
            'opacity': '1'      // Set opacity to 1 for the filter
        });

        $(".overlay").css({
            'z-index': '1',  // Ensure the overlay stays below the filter
            'opacity': '0.7'    // Adjust overlay opacity
        });

        $(id).on('click', function (element) {
            element.stopPropagation();
        });
        $(element).toggleClass('active');
    },


    /* this is used to hide filter on listing screen */
    hideFilter: function (self, id) {
        $(self).hide();
        $('.nt-sort, .nt-filter').removeClass('active');
    },

    /* this is used to open a drawer */
    openDrawer: function (self) {
        Common.showLoader();
        $(".drawer-inner", self).animate({ right: "0" });
        $("body").addClass('overflow-hidden');
        $(".overlay", self).css('display', 'block');
        Common.hideLoader();
    },

    /* this is used to close a drawer */
    closeDrawer: function (self) {
        Common.showLoader();
        $(".drawer-inner", self).animate({ right: "-100%" });
        $("body").removeClass('overflow-hidden');
        $(".overlay", self).css('display', 'none');
        Common.hideLoader();
    },

    /* this is used to show loader */
    showLoader: function () {
        $(".loading-wrapper").show();
    },

    /* this is used to hide loader */
    hideLoader: function () {
        $(".loading-wrapper").hide();
    },

    /* this is used to show project name edit option */
    ShowEditOption: function (element) {
        $(element).next(".edit-name").show();
    },

    /* this is used to hide project name edit option */
    CloseEditOption: function (element) {
        $(element).parents(".edit-name").hide();
    }
};

$(document).ready(function () {
    Common.init();
});