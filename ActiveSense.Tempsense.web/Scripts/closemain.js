

$('.main-search').hide();
$('button').click(function () {
    $('.main-search').show();
    $('.main-search text').focus();
}
);
$('.close').click(function () {
    $('.main-search').hide();
});