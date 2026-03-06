var timeout = 420;
var MenuCloseDelayTimer = null; //define as null initially to allow checking for null objects
var DropDownMenu = null;

// open (unhide) items for dropdown menu
function ddmOpen(id)
{
    // cancel the timout delay for closing the menu on mouse out
    cancelDelayedMenuClose();

    // close any possible previous open drop down menu
    if (DropDownMenu != null)
        DropDownMenu.style.visibility = 'hidden';
    // get new layer and show it
    DropDownMenu = document.getElementById(id);
    DropDownMenu.style.visibility = 'visible';
}

// close (rehide) previously opened dropdown
function mclose()
{
    if (DropDownMenu != null)
        DropDownMenu.style.visibility = 'hidden';
}

// If the user clicks outside of the menu hide any open menus
// document.onclick = mclose;

// start timer for close delay
function mclosetime()
{
    MenuCloseDelayTimer = window.setTimeout(mclose, timeout);
}

// clear close timer
function cancelDelayedMenuClose()
{
    if (MenuCloseDelayTimer != null)
    {
        window.clearTimeout(MenuCloseDelayTimer);
        MenuCloseDelayTimer = null;
    }
}

