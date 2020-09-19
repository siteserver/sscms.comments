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
  siteId: utils.getQueryInt('siteId') || $commentsConfigSiteId,
  channelId: utils.getQueryInt('channelId') || $commentsConfigChannelId,
  contentId: utils.getQueryInt('contentId') || $commentsConfigContentId,
  pageType: '',
  isSubmitDisabled: false,
  isApprovedByDefault: false,
  isCaptcha: false,
  captcha: '',
  captchaValue: '',
  captchaUrl: null,
  form: {},
  total: null,
  pageSize: null,
  total: null,
  page: 1,
  items: [],
  success: false
});

var methods = {
  apiGet: function (page) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        page: page
      }
    }).then(function (response) {
      var res = response.data;

      $this.isSubmitDisabled = res.isSubmitDisabled;
      $this.isApprovedByDefault = res.isApprovedByDefault;
      $this.isCaptcha = res.isCaptcha;
      if ($this.isCaptcha) {
        $this.apiCaptchaLoad();
      }
      $this.form = _.assign({
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentId: $this.contentId,
        captcha: '',
        content: ''
      });
      $this.pageType = 'form';

      $this.items = res.items;
      $this.pageSize = res.pageSize || 30;
      $this.total = res.total;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCaptchaLoad: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post('/v1/captcha').then(function (response) {
      var res = response.data;

      $this.captchaValue = res.value;
      $this.captchaUrl = $apiUrl + '/v1/captcha/' + res.value;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCaptchaCheck: function () {
    var $this = this;

    $api.post('/v1/captcha/actions/check', {
      captcha: this.form.captcha,
      value: this.captchaValue
    }).then(function (res) {
      $this.apiSubmit();
    })
    .catch(function (error) {
      utils.error(error);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      if ($this.isApprovedByDefault) {
        $this.items = res.items;
        $this.total = res.total;
      }
      
      $this.success = true;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        if ($this.isCaptcha) {
          $this.apiCaptchaCheck();
        } else {
          $this.apiSubmit();
        }
      }
    });
  },

  handleCurrentChange: function(val) {
    this.apiGet(val);
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet(1);
  }
});