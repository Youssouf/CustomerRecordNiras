
        var myCenter = new google.maps.LatLng(57.0416688, 9.9310923);

function initialize() {
    var mapProp = {
        center: myCenter, // new google.maps.LatLng(51.508742,-0.120850),
        zoom: 10,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

    var marker = new google.maps.Marker({
        position: myCenter,
        title : 'Click to zoom',

        animation: google.maps.Animation.BOUNCE
        // icon: 'pinkball.png'
    });
    marker.setMap(map);
    google.maps.event.addDomListener(marker, 'click', function () {
        map.setZoom(15);
        map.setCenter(marker.getPosition());
    });
}
google.maps.event.addDomListener(window, 'load', initialize);
