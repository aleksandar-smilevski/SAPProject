
$(function () {

    $('#categoryDialog').dialog({

        autoOpen: false,

        width: 400,

        height: 300,

        modal: true,

        title: 'Add Category',

        buttons: {

            'Save': function () {

                var createCategoryForm = $('#createCategoryForm');

                if (createCategoryForm.valid()) {

                    $.post(createCategoryForm.attr('action'), createCategoryForm.serialize(), function (data) {

                        if (data.Error != '') {

                            alert(data.Error);

                        }

                        else {

                            // Add the new genre to the dropdown list and select it

                            $('#Id').append(

                                    $('<option></option>')

                                        .val(data.Genre)

                                        .html(data.createCategoryForm.Name)

                                        .prop('selected', true)  // Selects the new Genre in the DropDown LB

                                );

                            $('#categoryDialog').dialog('close');

                        }

                    });

                }

            },

            'Cancel': function () {

                $(this).dialog('close');

            }

        }

    });
});

$('#categoryAddLink').click(function () {

    var createFormUrl = $(this).attr('href');  

    $('#categoryDialog').html('')

    .load(createFormUrl, function () {  

        jQuery.validator.unobtrusive.parse('#createCategoryForm');

        $('#categoryDialog').dialog('open');

    });

    return false;

});