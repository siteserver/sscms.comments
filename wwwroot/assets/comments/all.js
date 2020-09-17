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
  contentId: utils.getQueryInt('contentId') || $formConfigContentId,
  pageType: '',
  isClosed: false,
  isCaptcha: false,
  captchaUrl: null,
  captchaInValid: false,
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
        contentId: this.contentId,
        page: page
      }
    }).then(function (response) {
      var res = response.data;

      $this.isClosed = res.isClosed;
      $this.isCaptcha = res.isCaptcha;
      $this.form = _.assign({
        siteId: $this.siteId,
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

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      $this.items = res.items;
      $this.total = res.total;

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
        $this.apiSubmit();
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