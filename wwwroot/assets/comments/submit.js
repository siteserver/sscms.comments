$apiUrl = utils.getQueryString('apiUrl') || $formConfigApiUrl;
$rootUrl = "/";
$token = localStorage.getItem(ACCESS_TOKEN_NAME);

var $api = axios.create({
  baseURL: $apiUrl,
  headers: {
    Authorization: "Bearer " + $token,
  },
});

var $url = '/comments';

var data = utils.init({
  siteId: utils.getQueryInt('siteId') || $formConfigSiteId,
  pageType: '',
  styles: [],
  title: '',
  description: '',
  isCaptcha: false,
  captchaUrl: null,
  captchaInValid: false,
  uploadUrl: null,
  files: [],
  form: null,
});

var methods = {
  getUploadUrl: function(style) {
    return this.uploadUrl + '&fieldId=' + style.id;
  },

  imageUploaded: function(error, file) {
    if (!error) {
      var res = JSON.parse(file.serverId);
      var style = _.find(this.styles, function(o) { return o.id === res.fieldId; });
      style.value = res.value;
    }
  },

  imageRemoved: function(style) {
    style.value = [];
  },

  getForm: function(styles, value) {
    var form =  _.assign({}, value);
    for (var i = 0; i < styles.length; i++) {
      var style = styles[i];
      var name = _.lowerFirst(style.attributeName);
      if (style.inputType === 'TextEditor') {
        setTimeout(function () {
          var editor = UE.getEditor(style.attributeName, {
            allowDivTransToP: false,
            maximumWords: 99999999
          });
          editor.attributeName = style.attributeName;
          editor.ready(function () {
            editor.addListener("contentChange", function () {
              $this.form[this.attributeName] = this.getContent();
            });
          });
        }, 100);
      } else if (style.inputType === 'CheckBox' || style.inputType === 'SelectMultiple') {
        if (!form[name] || !Array.isArray(form[name])) {
          form[name] = [];
        }
      }
    }
    return form;
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/' + this.siteId + '/actions/getForm').then(function (response) {
      var res = response.data;

      $this.title = res.title;
      $this.description = res.description;
      $this.isCaptcha = res.isCaptcha;
      $this.styles = res.styles;
      $this.form = $this.getForm(res.styles, _.assign({
        captcha: ''
      }, res.comment));
      $this.pageType = 'form';
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url + '/' + this.siteId, _.assign({}, this.form)).then(function (response) {
      var res = response.data;

      $this.pageType = 'success';
      
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getValue: function (attributeName) {
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.attributeName === attributeName) {
        return style.value;
      }
    }
    return '';
  },

  setValue: function (attributeName, value) {
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.attributeName === attributeName) {
        style.value = value;
      }
    }
  },

  btnImageClick: function (imageUrl) {
    top.utils.openImagesLayer([imageUrl]);
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId,
      attributeName: options.attributeName
    };
    if (options.no) {
      query.no = options.no;
    }

    var args = {
      title: options.title,
      url: utils.getCommonUrl(options.name, query)
    };
    if (!options.full) {
      args.width = options.width ? options.width : 700;
      args.height = options.height ? options.height : 500;
    }
    utils.openLayer(args);
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});