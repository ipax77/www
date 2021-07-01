
window.LoadMap = () => {
    window.map = L.map('mapid', {
        maxBoundsViscosity: 1.0
    });
  window.map.on("load", loaded);
  window.map.setView([51.318008, 9.468067], 6);

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
  }).addTo(window.map);

  async function loaded() {
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
      L.polyline(latlngs, { color: mycolor }).addTo(window.map);
  } catch (error) {
    console.error(error);
  }
};

window.ClearLines = () => {
    for (i in window.map._layers) {
        if (window.map._layers[i]._path != undefined) {
            try {
                window.map.removeLayer(window.map._layers[i]);
            }
            catch (e) {
                console.log("problem with " + e + window.map._layers[i]);
            }
        }
    }
};

window.AddMarker = (latitude, longitude, info) => {
  var marker = new L.marker([latitude, longitude], {icon: markerIcon(0)});
  marker.alt = info;
  marker.bindPopup(info);
  marker.addTo(window.map);

  marker.on('click', function (event) {
      DotNet.invokeMethodAsync('www.pwa.Client', 'PointClick', marker.alt);
  });
  marker.on('mouseover', function(event){
      marker.openPopup();
  });
  marker.on('mouseout', function(event){
      marker.closePopup();
  });
};

function markerIcon(instId) {
  return new L.icon({
      iconUrl: 'images/marker-min.png',
      iconSize: [25, 40.5],
      iconAnchor: [12.5, 40.5],
      popupAnchor: [0, -40,5],
  });
};

window.FlyTo = (latitude, longitude, zoom) => {
    window.map.flyTo([latitude, longitude], zoom);
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

  function GetWidth(id) {
      var elmnt = document.getElementById(id);
      if (elmnt != null) {
          return elmnt.clientWidth;
      } else {
          return 0;
      }
  }