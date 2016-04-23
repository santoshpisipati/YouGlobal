/**
 * Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    config.skins = 'v2';
    config.toolbar = 'toolbarLight';
    config.MaxLength = 10000;
    config.extraPlugins = 'onchange';
    config.toolbar_toolbarLight =
    [
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Scayt'],
        ['Undo', 'Redo', '-', 'Find', 'Replace', '-', 'SelectAll', 'RemoveFormat'],
        ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'Link', 'Unlink', 'Anchor', 'Maximize'],
        '/',
        ['Styles', 'Format', 'FontSize', 'Bold', 'Italic', 'Strike', 'NumberedList', 'BulletedList', 'Outdent', 'Indent', 'Blockquote', 'TextColor', 'BGColor'],
    ];
    config.wordcount = {
        // Whether or not you want to show the Paragraphs Count
        showParagraphs: true,
        // Whether or not you want to show the Word Count
        showWordCount: true,
        // Whether or not you want to show the Char Count
        showCharCount: true,
        // Whether or not you want to count Spaces as Chars
        countSpacesAsChars: false,
        // Whether or not to include Html chars in the Char Count
        countHTML: false,
        // Maximum allowed Word Count, -1 is default for unlimited
        maxWordCount: -1,
        // Maximum allowed Char Count, -1 is default for unlimited
        maxCharCount: -1,
        // Add filter to add or remove element before counting (see CKEDITOR.htmlParser.filter), Default value : null (no filter)
        filter: new CKEDITOR.htmlParser.filter({
            elements: {
                div: function (element) {
                    if (element.attributes.class == 'mediaembed') {
                        return false;
                    }
                }
            }
        })
    };
};