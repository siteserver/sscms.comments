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