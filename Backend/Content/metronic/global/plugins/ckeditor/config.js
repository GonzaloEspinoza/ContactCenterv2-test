/**
 * @license Copyright (c) 2003-2014, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */
CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';

    toolbarGroups: [
    { name: 'document', groups: ['mode', 'document'] },			// Displays document group with its two subgroups.
    { name: 'clipboard', groups: ['clipboard', 'undo'] },			// Group's name will be used to create voice label.
    '/',																// Line break - next group will be placed in new line.
    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
    { name: 'links' }
    ]

    config.removeButtons = 'Underline,Subscript,Superscript';
};


