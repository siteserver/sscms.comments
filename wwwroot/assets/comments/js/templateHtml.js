var $url = '/comments/templateHtml';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  type: utils.getQueryString('type'),
  name: utils.getQueryString('name'),
  templateInfo: null,
  templateHtml: null,
  isSystem: false,
  contentEditor: null,
});

var methods = {
  apiGet: function () {
    var $this = this;
    
    utils.loading(this, true);
    $api.get($url, {
      params: {
        name: this.name
      }
    }).then(function (response) {
      var res = response.data;

      $this.templateInfo = res.templateInfo;
      $this.templateHtml = res.templateHtml;
      $this.isSystem = res.isSystem;

      setTimeout(function () {
        $('#templateContent').height($(document).height() - 150);
        require.config({ paths: { 'vs': utils.getAssetsUrl('lib/monaco-editor/min/vs') }});
        require(['vs/editor/editor.main'], function() {
          $this.contentEditor = monaco.editor.create(document.getElementById('templateContent'), {
              value: $this.templateHtml,
              language: 'html'
          });
          $this.contentEditor.focus();
        });
      }, 100);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getEditorContent: function() {
    return this.contentEditor.getModel().getValue();
  },

  btnSubmitClick: function (isClose) {
    if (this.isSystem) {
      var url = utils.getRootUrl('comments/templatesLayerEdit', {
        siteId: this.siteId,
        type: this.type,
        name: this.name,
        isSystem: true
      });

      utils.openLayer({
        title: '克隆模板',
        url: url
      });
      return;
    }

    var $this = this;
    this.templateHtml = this.getEditorContent();
    utils.loading(this, true);
    $api.post($url, {
      name: this.name,
      templateHtml: this.templateHtml
    }).then(function (response) {
      var res = response.data;

      utils.success('模板编辑成功！');
      if (isClose) {
        utils.removeTab();
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
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