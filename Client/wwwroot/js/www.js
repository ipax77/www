const geooptions = {
  enableHighAccuracy: true,
  maximumAge: 0,
  timeout: Infinity
};


window.GetLocation = () => {
  if(!navigator.geolocation) {
    return 'Geolocation is not supported by your browser';
  } else {
    navigator.geolocation.getCurrentPosition(success, error, geooptions);
    return '';
  }

  async function success(position) {
    // var speed = position.coords.speed;
    // if (speed == null)
    //   speed = 0;
    await DotNet.invokeMethodAsync('www.pwa.Client', 'UpdateRunCaller', [position.coords.latitude, position.coords.longitude, position.timestamp, position.coords.accuracy, 0]);
  }

  async function error(err) {
    await DotNet.invokeMethodAsync('www.pwa.Client', 'ErrorCaller', 'ERROR(' + err.code + '): ' + err.message);
  }
};

window.GetLocationDebug = () => {

  const geooptionsdebug = {
    enableHighAccuracy: true,
    maximumAge: 0,
    timeout: Infinity
  };

  if(!navigator.geolocation) {
    return 'Geolocation is not supported by your browser';
  } else {
    navigator.geolocation.getCurrentPosition(success, error, geooptionsdebug);
    return '';
  }

  async function success(position) {
    // var speed = position.coords.speed;
    // if (speed == null)
    //   speed = 0;
    await DotNet.invokeMethodAsync('www.pwa.Client', 'UpdateRunCallerDebug', [position.coords.latitude, position.coords.longitude, position.timestamp, position.coords.accuracy, 0]);
  }

  async function error(err) {
    await DotNet.invokeMethodAsync('www.pwa.Client', 'ErrorCallerDebug', 'ERROR(' + err.code + '): ' + err.message);
  }
};

// window.StartRun = async() => {

//   const options = {
//     enableHighAccuracy: true,
//     maximumAge: 30000,
//     timeout: 27000
//   };

//   if(!navigator.geolocation) {
//     return 'Geolocation is not supported by your browser';
//   } else {
//     const watchId = navigator.geolocation.watchPosition(success, error, options);
//     // const watchId = navigator.geolocation.watchPosition(success, error);
//     return String(watchId);
//   }

//   async function success(position) {
//     var speed = position.coords.speed;
//     if (speed == null)
//       speed = 0;
//     await DotNet.invokeMethodAsync('www.pwa.Client', 'UpdateRunCaller', [position.coords.latitude, position.coords.longitude, position.timestamp, position.coords.accuracy, speed]);
//   }

//   async function error(err) {
//     await DotNet.invokeMethodAsync('www.pwa.Client', 'ErrorCaller', 'ERROR(' + err.code + '): ' + err.message);
//   }

// };

// window.StopRun = (watchId) => {
//   navigator.geolocation.clearWatch(watchId);
// };

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
      // window.map.flyToBounds(polyline.getBounds());
    }
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

window.Geo  = {

  remind: function() {
    this.count = this.count + 1;
    // alert(this.count);
    // this.timeoutID = undefined;
    // console.log(this.count);
    var ele = document.getElementById("geocount");
    if (ele != null)
      ele.innerHTML = this.count.toString();
    this.setup(this.count);
  },

  setup: function(count = 0) {
    if (typeof this.timeoutID === 'number') {
      this.cancel();
    }
    this.count = count;
    this.timeoutID = window.setTimeout(function() {
      this.remind();
    }.bind(this), 1000);
  },

  cancel: function() {
    window.clearTimeout(this.timeoutID);
  }
};

window.StartRunService = () => {
  var ele = document.getElementById('geocount');
  ele.innerHTML = '0';
  window.Geo.setup();
};

window.StopRunService = () => {
  window.Geo.cancel();
}