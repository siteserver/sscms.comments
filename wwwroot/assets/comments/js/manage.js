var $url = '/comments/manage';
var $urlExport = $url + '/actions/export';
var $urlSetStatus = $url + '/actions/setStatus';
var $urlDelete = $url + '/actions/delete';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  status: 'All',
  keyword: '',
  total: null,
  pageSize: null,
  total: null,
  page: 1,
  items: [],
  multipleSelection: [],
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
        status: this.status,
        keyword: this.keyword,
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

  apiDelete: function (comment) {
    var $this = this;

    utils.loading(true);
    $api.post($urlDelete, {
      siteId: this.siteId,
      channelId: this.channelId,
      contentId: this.contentId,
      status: this.status,
      keyword: this.keyword,
      commentId: comment.id
    }).then(function (response) {
      var res = response.data;

      $this.items = res.items;
      $this.total = res.total;
      utils.success('评论删除成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSetStatus: function (status) {
    var $this = this;

    utils.loading(true);
    $api.post($urlSetStatus, {
      siteId: this.siteId,
      channelId: this.channelId,
      contentId: this.contentId,
      status: this.status,
      keyword: this.keyword,
      commentIds: this.commentIds,
      commentStatus: status
    }).then(function (response) {
      var res = response.data;

      $this.items = res.items;
      $this.total = res.total;
      utils.success('评论状态设置成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  tableRowClassName: function(scope) {
    if (this.multipleSelection.indexOf(scope.row) !== -1) {
      return 'current-row';
    }
    return '';
  },

  handleSelectionChange: function(val) {
    this.multipleSelection = val;
  },

  toggleSelection: function(row) {
    this.$refs.multipleTable.toggleRowSelection(row);
  },

  btnSearchClick: function() {
    this.apiGet(1);
  },

  handleCurrentChange: function(val) {
    this.apiGet(val);
  },

  btnStatusClick: function(status) {
    if (!this.isCommentChecked) return;
    this.apiSetStatus(status);
  },

  getContentUrl: function (comment) {
    return utils.getRootUrl('redirect', {
      siteId: comment.siteId,
      channelId: comment.channelId,
      contentId: comment.contentId
    });
  },

  getStatus: function(status) {
    if (status === 'Approved') return '已审核';
    else if (status === 'Spam') return '垃圾信息';
    else return '待审核';
  },

  getStatusType: function(status) {
    if (status === 'Approved') return 'primary';
    else if (status === 'Spam') return 'warning';
    else return 'danger';
  },

  btnEditClick: function (comment) {
    utils.openLayer({
      title: '编辑评论',
      width: 550,
      height: 300,
      url: utils.getRootUrl('comments/manageLayerEdit', {
        siteId: this.siteId,
        channelId: comment.channelId,
        contentId: comment.contentId,
        commentId: comment.id
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

  btnDeleteClick: function (comment) {
    var $this = this;

    utils.alertDelete({
      title: '删除评论',
      text: '此操作将删除评论，确定吗？',
      callback: function () {
        $this.apiDelete(comment);
      }
    });
  },

  btnExportClick: function () {
    var $this = this;
    utils.loading(true);

    $api.post($urlExport, {
      siteId: this.siteId,
      channelId: this.channelId,
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
  computed: {
    isCommentChecked: function() {
      return this.multipleSelection.length > 0;
    },

    commentIds: function() {
      var retVal = [];
      for (var i = 0; i < this.multipleSelection.length; i++) {
        var comment = this.multipleSelection[i];
        retVal.push(comment.id);
      }
      return retVal;
    }
  },
  created: function () {
    this.apiGet(1);
  }
});
