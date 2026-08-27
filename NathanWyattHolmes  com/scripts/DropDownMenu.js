var timeout = 420;
var MenuCloseDelayTimer = null; //define as null initially to allow checking for null objects
var DropDownMenu = null;
var lastTouchTime = 0;

// Track touch interactions to differentiate real mouse hover from mobile touch emulation
window.addEventListener('touchstart', function()
{
    lastTouchTime = Date.now();
}, { passive: true, capture: true });

function isRecentTouch()
{
    return (Date.now() - lastTouchTime) < 700;
}

// open (unhide) items for dropdown menu on desktop mouse hover
function ddmOpen(id)
{
    // On touch devices, ignore synthesized mouseover to prevent race conditions with click/tap
    if (isRecentTouch())
    {
        return;
    }

    // cancel the timeout delay for closing the menu on mouse out
    cancelDelayedMenuClose();

    var target = document.getElementById(id);
    if (!target) return;

    // close any possible previous open drop down menu
    if (DropDownMenu != null && DropDownMenu !== target)
        DropDownMenu.style.visibility = 'hidden';

    // get new layer and show it
    DropDownMenu = target;
    DropDownMenu.style.visibility = 'visible';
}

// toggle dropdown open/closed (ideal for touch/mobile devices and clicks)
function ddmToggle(id, event)
{
    if (event)
    {
        event.stopPropagation();
        if (event.preventDefault)
        {
            event.preventDefault();
        }
    }
    cancelDelayedMenuClose();

    var target = document.getElementById(id);
    if (!target) return;

    if (DropDownMenu === target && target.style.visibility === 'visible')
    {
        target.style.visibility = 'hidden';
        DropDownMenu = null;
    }
    else
    {
        if (DropDownMenu != null && DropDownMenu !== target)
            DropDownMenu.style.visibility = 'hidden';

        DropDownMenu = target;
        DropDownMenu.style.visibility = 'visible';
    }
}

// close (rehide) previously opened dropdown
function mclose()
{
    if (DropDownMenu != null)
    {
        DropDownMenu.style.visibility = 'hidden';
        DropDownMenu = null;
    }
}

// start timer for close delay
function mclosetime()
{
    // On touch devices, do not schedule timeout on touch synthesized mouseout
    if (isRecentTouch())
    {
        return;
    }

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

// Close dropdown if user clicks or taps outside the menu
document.addEventListener('click', function(e)
{
    var menu = document.getElementById('TopDropDownMenu');
    if (menu && !menu.contains(e.target))
    {
        mclose();
    }
});

