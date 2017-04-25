/**
 * @license Copyright (c) 2003-2016, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';

    //是否强制复制来的内容去除格式 plugins/pastetext/plugin.js   
    config.forcePasteAsPlainText = false//不去除  
    //是否使用等标签修饰或者代替从word文档中粘贴过来的内容 plugins/pastefromword/plugin.js    
    config.pasteFromWordKeepsStructure = false;
    //从word中粘贴内容时是否移除格式 plugins/pastefromword/plugin.js  
    config.pasteFromWordRemoveStyle = false
    config.pasteFromWordRemoveFontStyles = false;
};
