document.addEventListener("DOMContentLoaded", () => {
    document.body.addEventListener("click", (e) => {
        if (e.target?.classList?.contains("bi-arrows-fullscreen") || e.target?.previousElementSibling?.classList?.contains("bi-arrows-fullscreen") || e.target?.id == "FullScreen") {
            e.preventDefault();
            if (document.fullscreenEnabled) {
                if (document.fullscreenElement) document.exitFullscreen();
                else document.getElementsByTagName("html")[0].requestFullscreen(); //document.body.requestFullscreen();
            }
        }
        if (e.target?.classList?.contains("bi-eyeglasses") || e.target?.previousElementSibling?.classList?.contains("bi-eyeglasses") || e.target?.id == "VisuallyImpaired") {
            e.preventDefault();
            if (document.body.style.zoom) {
                document.body.style.zoom = null;
                window.onresize();
            }
            else {
                document.body.style.zoom = 1.4;
                window.onresize("reset");
            }
        }
        if (e.target?.classList?.contains("bi-moon-stars-fill") || e.target?.previousElementSibling?.classList?.contains("bi-moon-stars-fill") || e.target?.id == "SwitchTheme") {
            e.preventDefault();
            let theme = document.getElementsByTagName("html")[0].dataset["bsTheme"];
            if (theme == "light") document.getElementsByTagName("html")[0].dataset["bsTheme"] = "dark";
            else document.getElementsByTagName("html")[0].dataset["bsTheme"] = "light";
        }
    }, false);
});
window.onresize = (e) => {
    let size = window.innerHeight;
    let main = document.getElementById("HomeMain");
    if (main && size) {
        let h = main?.clientHeight;
        if (h >= size || e == "reset" || document.body.style.zoom) main.style.marginTop = null;
        else main.style.marginTop = ((size - h) / 4) + "px";
    }
};
window.RefreshRecommendation = () => {
    let btn = document.getElementById('RefreshButton');
    if (btn && btn.firstElementChild.className == 'bi bi-heart-fill') btn.firstElementChild.className = 'spinner-border';
    else btn.firstElementChild.className = 'bi bi-heart-fill';
};
