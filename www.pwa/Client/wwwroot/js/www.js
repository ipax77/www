
window.LoadMap = () => {
  console.log("loading map..");

  window.map = L.map('mapid');
  window.map.on("load", loaded);
  window.map.setView([51.318008, 9.468067], 6);

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
  }).addTo(window.map);

  async function loaded() {
    console.log("map loaded");
    await DotNet.invokeMethodAsync('www.pwa.Client', 'MapLoadedCaller');
  }

  return true;
};

window.AddLine = (latlngs, mycolor) => {
  try {
    var polyline = L.polyline(latlngs, {color: mycolor}).addTo(window.map);
    
    if (polyline != null) {
      window.map.fitBounds(polyline.getBounds());
      window.map.flyToBounds(polyline.getBounds());
    }
  } catch (error) {
    console.error(error);
  }
};

window.AddCurrentLine = (latlngs, mycolor) => {
  try {
    L.polyline(latlngs, {color: mycolor}).addTo(window.map);
  } catch (error) {
    console.error(error);
  }
};

window.AddMarker = (latitude, longitude, accuracy, index) => {
  var marker = new L.marker([latitude, longitude], {icon: markerIcon(index)});
  marker.alt = index.toString();
  marker.bindPopup(index + '|' + latitude + '|' + longitude + ' (' + accuracy + ')');
  marker.addTo(window.map);
  
  marker.on('mouseover', function(event){
      marker.openPopup();
  });
  marker.on('mouseout', function(event){
      marker.closePopup();
  });
};

function markerIcon(instId) {
  return new L.icon({
      iconUrl: 'images/marker1-min.png',
      iconSize: [35, 35]
      // iconAnchor: [22, 94],
      // popupAnchor: [-3, -76],
  });
};

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

function Scroll(id)
{
    var elmnt = document.getElementById(id);
    if (elmnt != null) {
        elmnt.scrollIntoView({ behavior: 'smooth' });
    }
};