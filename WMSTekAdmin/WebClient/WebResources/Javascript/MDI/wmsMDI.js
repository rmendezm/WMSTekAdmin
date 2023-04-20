// Show modal dialog
function showDialog(winTitle, winUrl) {
    contentWin = new HWWeb("winModalDialog");

    contentWin.showDialog({ title: winTitle, url: winUrl });
}   

var DefaultClassObject = {
    create: function() {
        return function() {
            this.initialize.apply(this, arguments);
        }
    }
}

// Object utility to create and manipulate the Windows
var HWWeb = DefaultClassObject.create();

HWWeb.prototype = {

    themeName: "yellowlighting",
    blurThemeName: "bluelighting",
    width: 400,
    height: 200,
    top: 0,
    left: 0,
    winObj: null,
    winID: "",
    title: "",
    url: "http://about:blank/",


    // Constructor
    initialize: function(id) {
        this.winID = id;
    },

    showWindow: function(oArg) {
        this.width = oArg.width || this.width;
        this.height = oArg.height || this.height;
        this.left = oArg.left || this.left;
        this.top = oArg.top || this.top;
        this.themeName = oArg.themeName || this.themeName;
        this.title = oArg.title || this.title;
        this.titlePath = oArg.titlePath || this.titlePath;
        this.url = oArg.url || this.url;
        this.blurThemeName = oArg.blurThemeName || this.blurThemeName;


        this.winObj = new Window(this.winID, { className: this.themeName, blurClassName: this.blurThemeName, title: this.title,
        titlePath: this.titlePath, top: this.top, left: this.left, width: this.width, height: this.height,
            closable: true, url: this.url, hideEffectOptions: { duration: 0.2 }, showEffectOptions: { duration: 0.2 }
        });

        this.winObj.setDestroyOnClose();
        this.winObj.setConstraint(true, { top: 42, bottom: 21, left: 0, right: 0 });
        this.winObj.show();
        this.winObj.toFront();

        // Create a dock element
        var element = document.createElement("span");
        element.className = "dock_icon_focus";
        element.style.display = "none";
        element.win = this.winObj;
        this.winObj.statusBar = element;
        $('dock').appendChild(element);
        Event.observe(element, "mouseup", Windows.restore);

        // Check win title length
        var maxTitleLenght = 16;

        //        if (this.title.length > maxTitleLenght)
        //            this.title = oArg.shortTitle;

        if (this.title.length > maxTitleLenght)
            this.title = this.title.substring(0, maxTitleLenght) + '..';

        //$(element).update(oArg.shortTitle);
        $(element).update(this.title);

        new Effect.Appear(element);
        // END Create a dock element

    },

    showDialog: function(oArg) {
        this.width = oArg.width || this.width;
        this.height = oArg.height || this.height;
        this.left = oArg.left || this.left;
        this.top = oArg.top || this.top;
        this.themeName = oArg.theme || this.themeName;
        this.title = oArg.title || this.title;
        this.url = oArg.url || this.url;


        this.winObj = new Window(this.winID, { className: this.themeName, title: this.title,
            top: this.top, right: this.left, width: this.width, height: this.height,
            resizable: true, url: this.url
        });

        this.winObj.setDestroyOnClose();

        this.winObj.show(true);
    },

    findWindow: function(id) {
        return (Windows.getWindow(id));

    },

    findWindowFrom: function(id, from) {
        return (from.Windows.getWindow(id));

    }
    ,
    toFront: function() {
        this.winObj.toFront();
    }
    ,
    maximize: function() {
        this.winObj.maximize();
    }
}

// Overide Windows minimize to move window inside dock
Object.extend(Windows, {
    // Overide minimize function
    minimize: function(id, event) {
        var win = this.getWindow(id)
        if (win && win.visible) {
            // Hide current window
            win.hide();
        }
        Event.stop(event);
    },
 
    // Restore function
    restore: function(event) {
        var element = Event.element(event);
        // Show window
        element.win.show();
        Windows.focus(element.win.getId());
        element.win.toFront();

        element.className = "dock_icon_focus";
    }
})

// blur focused window if click on document
Event.observe(document, "click", function(event) {   
  var e = Event.element(event);
  var win = e.up(".dialog");
  var dock = e == $('dock') || e.up("#dock"); 
  if (!win && !dock && Windows.focusedWindow) {
    Windows.blur(Windows.focusedWindow.getId());                    
  }
})               

// Change theme callback
var currentTheme = 0;
function changeTheme(event) {
  var index = Event.element(event).selectedIndex;
  if (index == currentTheme)
    return;

  var theme, blurTheme;
  switch (index) {
    case 0:
      theme = "mac_os_x";
      blurTheme = "blur_os_x";
      break;
    case 1:
      theme = "yellowlighting";
      blurTheme = "bluelighting";
      break;
    case 2:
      theme = "greenlighting";
      blurTheme = "greylighting";
      break;
  }
  Windows.windows.each(function(win) {
    win.options.focusClassName = theme; 
    win.options.blurClassName = blurTheme;
    win.changeClassName(blurTheme)
  });
  Windows.focusedWindow.changeClassName(theme);
  currentTheme = index;
}

// Check if a window exist
function isWinExist(id) {
    var exist = false;
    var win = Windows.getWindow(id);
    // Asks delegate if exists
    if (win) {
        exist = true;
    }

    return (exist);
}

// Close all Windows
function closeAllWindows() {
    Windows.closeAll();
}

// Minimize all Windows
function minimizeAllWindows() {
    Windows.minimizeAll();
}

// Minimize Size All Windows
var clickMinimize = false;
function minimizeSizeAllWindows(obj) {
    Windows.minimizeSizeAll();
    
    if (Windows.windows.length > 0) {
        if (!clickMinimize) {
            obj.title = "Maximizar Tama\u00f1o";
            clickMinimize = true;
        } else {
            obj.title = "Minimizar Tama\u00f1o";
            clickMinimize = false;
        }
    } else {
        obj.title = "Minimizar Tama\u00f1o";
    }
}

// Close all modals Windows
function closeAllModalWindows() {
    Windows.closeAllModalWindows();
}

// Tile all Windows
function tileAllWindows(xRatio, yRation) {
    var x = xRatio;
    var y = yRation;

    Windows.windows.each(function(w) {
        if (w.getId != null) {
            w.setLocation(x, y);
            w.toFront();
            x += xRatio;
            y += yRation;
        }
    });
}

// Main MDI Javascript Section
var contentWin = null;

// Open Window
function openWindow(winName, winTitle, winPath, winShortTitle, winUrl, maxOpenedWin, maxOpenedWinMessage) {
    count = 0;
    
    // Cuenta la cantidad de ventanas abiertas
    Windows.windows.each(function(w) { count += 1;});

    // Si no existe, la crea
    if (!isWinExist(winName)) {
    
        // Verifica que no se habran mas ventanas que la cantidad maxima configurada
        if (count < maxOpenedWin) {
            contentWin = new HWWeb(winName);
            contentWin.showWindow({ title: winTitle, titlePath: winPath, shortTitle: winShortTitle, themeName: 'yellowlighting', url: winUrl });
            setTimeout("contentWin.maximize()", 10);
        }
        else {
            alert(maxOpenedWinMessage);
        }
    }
    // Si ya existe, hace foco en la pagina solicitada
    else {
        win = Windows.getWindow(winName);
        win.show();
        Windows.focus(win.getId());
        win.toFront();
    }
}

// Tile all Windows
function tileWindows() {
    tileAllWindows(50, 20);
}

 
// FIN Main MDI Javascript Section    