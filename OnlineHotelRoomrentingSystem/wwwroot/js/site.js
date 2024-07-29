function toggleRooms() {
    var roomsSection = document.getElementById("roomsSection");
    var toggleButton = document.getElementById("toggleRoomsButton");
    if (roomsSection.style.display === "none") {
        roomsSection.style.display = "block";
        toggleButton.textContent = "Hide Rooms";
    } else {
        roomsSection.style.display = "none";
        toggleButton.textContent = "Show Rooms";
    }
}