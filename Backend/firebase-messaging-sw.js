importScripts("https://www.gstatic.com/firebasejs/6.3.4/firebase-app.js");
importScripts("https://www.gstatic.com/firebasejs/6.3.4/firebase-messaging.js");

// Initialize Configuration
var firebaseConfig = {
    apiKey: "AIzaSyDuETtPsP297zh8N5VVLPrwc8nunSxWWek",
    authDomain: "yaigo-1522251423246.firebaseapp.com",
    databaseURL: "https://yaigo-1522251423246.firebaseio.com",
    projectId: "yaigo-1522251423246",
    storageBucket: "yaigo-1522251423246.appspot.com",
    messagingSenderId: "933243260335",
    appId: "1:933243260335:web:3b3bf0e302554561"
};
firebase.initializeApp(firebaseConfig);

const messaging = firebase.messaging();
//messaging.usePublicVapidKey("BDHOTO5FJNjhmeHWOzppDjzV5V82vz7bJ2vsy1G4tMQAp3_nqJ_Xw313cnF7sr162UFn4bAMI0jfgCTpIR93P0E");