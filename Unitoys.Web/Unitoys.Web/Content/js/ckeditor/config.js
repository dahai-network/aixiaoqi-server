/**
 * @license Copyright (c) 2003-2016, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';

    //�Ƿ�ǿ�Ƹ�����������ȥ����ʽ plugins/pastetext/plugin.js   
    config.forcePasteAsPlainText = false//��ȥ��  
    //�Ƿ�ʹ�õȱ�ǩ���λ��ߴ����word�ĵ���ճ������������ plugins/pastefromword/plugin.js    
    config.pasteFromWordKeepsStructure = false;
    //��word��ճ������ʱ�Ƿ��Ƴ���ʽ plugins/pastefromword/plugin.js  
    config.pasteFromWordRemoveStyle = false
    config.pasteFromWordRemoveFontStyles = false;
};
