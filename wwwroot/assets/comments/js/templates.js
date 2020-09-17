var $url = '/comments/templates';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  templateInfoList: null,
  name: null,
  templateHtml: null,
});

var methods = {
  getIconUrl: function (templateInfo) {
    return '/assets/comments/templates/' + templateInfo.name + '/' + templateInfo.icon;
  },

  getCode: function (templateInfo) {
    return '<stl:comments type="' + templateInfo.name + '"></stl:comments>';
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.templateInfoList = res.templateInfoList;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnEditClick: function (name) {
    var url = utils.getRootUrl('comments/templatesLayerEdit', {
      siteId: this.siteId,
      name: name
    });
    utils.openLayer({
      title: '模板设置',
      url: url
    });
  },

  btnHtmlClick: function (templateInfo) {
    var url = utils.getRootUrl('comments/templateHtml', {
      siteId: this.siteId,
      name: templateInfo.name
    });
    utils.addTab('代码编辑', url);
  },

  btnDeleteClick: function (template) {
    var $this = this;
    utils.alertDelete({
      title: '删除模板',
      text: '此操作将删除模板' + template.name + '，确认吗？',
      callback: function () {
        utils.loading(true);
        $api.delete($url, {
          data: {
            siteId: $this.siteId,
            name: template.name
          }
        }).then(function (response) {
          var res = response.data;

          $this.templateInfoList = res.templateInfoList;
        }).catch(function (error) {
          utils.error(error);
        }).then(function () {
          utils.loading($this, false);
        });
      }
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    utils.loading(true);
    $api.post($url + '?siteId=' + this.siteId, {
      name: this.name,
      templateHtml: this.templateHtml
    }).then(function (response) {
      var res = response.data;

      utils.success('模板编辑成功！');
      $this.pageType = 'list';
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnNavClick: function(type) {
    utils.loading(true);
    var url = utils.getRootUrl('comments/templates', {
      siteId: this.siteId,
      type: type
    });
    location.href = url;
  },

  btnPreviewClick: function(template) {
    var url = '/assets/comments/templates/' + template.name + '/index.html?siteId=' + this.siteId + '&apiUrl=' + encodeURIComponent('/api');
    window.open(url);
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