var $url = '/comments/settings';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  settings: null,
  styleList: [],
  attributeNames: null,
  administratorSmsNotifyKeys: null,
  userSmsNotifyKeys: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.settings = res.settings;
      if ($this.settings.pageSize === 0) {
        $this.settings.pageSize = 30;
      }
      $this.styleList = res.styleList;
      $this.attributeNames = res.attributeNames;
      $this.administratorSmsNotifyKeys = res.administratorSmsNotifyKeys;
      $this.userSmsNotifyKeys = res.userSmsNotifyKeys;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.settings).then(function (response) {
      var res = response.data;

      utils.success('设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.settings.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  getAttributeText: function (attributeName) {
    if (attributeName === 'AddDate') {
      return '添加时间';
    }
    return attributeName;
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
