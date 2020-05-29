const fs = require('fs')
const path = require('path')

exports.devroot = () => {
  const pagePath = path.resolve(__dirname, '../../../Views/VueSinglePage.cshtml')
  fs.writeFileSync(pagePath, `
          @{
              ViewBag.Title = "Index";
              Layout = null;

        }
        <div id="app"></div>
        <script src="http://localhost:8081/app.js"></script>
    `, 'utf8')
}

exports.prodroot = () => {
  const pagePath = path.resolve(__dirname, '../../../Views/VueSinglePage.cshtml')
  const nowTime = new Date().getTime()
  fs.writeFileSync(pagePath, `
            @{
                ViewBag.Title = "Index";
                Layout = null;
  
          }
          <link href="/Scripts/static/css/app.css?v='${nowTime}'" rel=stylesheet>
          <div id="app"></div>
          <script type=text/javascript src="/Scripts/static/js/manifest.js?v='${nowTime}'"></script>
          <script type=text/javascript src="/Scripts/static/js/vendor.js?v='${nowTime}'"></script>
          <script type=text/javascript src="/Scripts/static/js/app.js?v='${nowTime}'"></script>
      `, 'utf8')
}
