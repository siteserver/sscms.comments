var $url = '/comments/templatesLayerEdit';
var $urlUpdate = $url + '/actions/update';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  type: utils.getQueryString('type'),
  name: utils.getQueryString('name'),
  isSystem: utils.getQueryBoolean('isSystem'),
  templateInfo: null
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
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getTemplateHtml: function() {
    return parent.$vue.getEditorContent();
  },

  apiClone: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      type: this.type,
      originalName: this.name,
      name: this.templateInfo.name,
      description: this.templateInfo.description,
      templateHtml: this.getTemplateHtml()
    }).then(function (response) {
      utils.success('模板克隆成功！');
      parent.location.href = $this.getTemplatesUrl();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiEdit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlUpdate, {
      type: this.type,
      originalName: this.name,
      name: this.templateInfo.name,
      description: this.templateInfo.description
    }).then(function (response) {

      utils.success('模板编辑成功！');
      parent.location.href = $this.getTemplatesUrl();
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
        if ($this.isSystem) {
          $this.apiClone();
        } else {
          $this.apiEdit();
        }
      }
    });
  },

  getTemplatesUrl: function() {
    return utils.getRootUrl('comments/templates', {
      siteId: this.siteId,
      type: this.type
    });
  },

  btnCancelClick: function() {
    window.parent.layer.closeAll()
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
