var $url = '/comments/manage';
var $urlActionsExport = '/comments/manage/actions/export';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  contentId: utils.getQueryInt('contentId'),
  total: null,
  pageSize: null,
  total: null,
  page: 1,
  items: []
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

      $this.items = res.items;
      $this.pageSize = res.pageSize || 30;
      $this.total = res.total;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (commentId) {
    var $this = this;

    utils.loading(true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        contentId: this.contentId,
        commentId: commentId
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.items;
      $this.total = res.total;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  handleCurrentChange: function(val) {
    this.apiGet(val);
  },

  btnEditClick: function (commentId) {
    utils.openLayer({
      title: '编辑评论',
      width: 550,
      height: 300,
      url: utils.getRootUrl('comments/manageLayerEdit', {
        siteId: this.siteId,
        contentId: this.contentId,
        commentId: commentId
      })
    });
  },

  btnViewClick: function(userId) {
    if (userId === 0) return;
    
    utils.openLayer({
      title: '查看资料',
      url: utils.getCommonUrl('userLayerView', {userId: userId})
    });
  },

  btnDeleteClick: function (commentId) {
    var $this = this;

    utils.alertDelete({
      title: '删除评论',
      text: '此操作将删除评论，确定吗？',
      callback: function () {
        $this.apiDelete(commentId);
      }
    });
  },

  btnExportClick: function () {
    var $this = this;
    utils.loading(true);

    $api.post($urlActionsExport, {
      siteId: this.siteId,
      contentId: this.contentId,
    }).then(function (response) {
      var res = response.data;

      utils.success('数据导出成功！');
      window.open(res.value);

    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet(1);
  }
});
