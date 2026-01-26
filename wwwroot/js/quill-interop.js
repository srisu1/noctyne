// Quill.js Integration for MoodJournal - FIXED VERSION
// Place this file in: wwwroot/js/quill-interop.js

let quillInstance = null;
let isInitializing = false;

/**
 * Initialize Quill editor with proper configuration
 * Returns a promise that resolves when initialization is complete
 */
window.initQuillEditor = function() {
    return new Promise((resolve, reject) => {
        try {
            // If already initializing, wait for it
            if (isInitializing) {
                console.log('Quill already initializing, waiting...');
                const checkInterval = setInterval(() => {
                    if (quillInstance && !isInitializing) {
                        clearInterval(checkInterval);
                        resolve(quillInstance);
                    }
                }, 50);
                return;
            }

            // If already initialized, destroy and recreate
            if (quillInstance) {
                console.log('Destroying existing Quill instance');
                quillInstance = null;
            }

            isInitializing = true;

            // Wait for DOM to be ready
            setTimeout(() => {
                const editorElement = document.getElementById('quill-editor');
                const toolbarElement = document.getElementById('quill-toolbar');

                if (!editorElement || !toolbarElement) {
                    isInitializing = false;
                    reject(new Error('Quill editor elements not found'));
                    return;
                }

                console.log('Initializing Quill editor...');

                quillInstance = new Quill('#quill-editor', {
                    modules: {
                        toolbar: '#quill-toolbar'
                    },
                    theme: 'snow',
                    placeholder: 'How are you feeling today? What\'s on your mind?'
                });

                // Listen for text changes
                quillInstance.on('text-change', function() {
                    // Trigger word count update if needed
                    if (window.updateWordCount) {
                        window.updateWordCount();
                    }
                });

                isInitializing = false;
                console.log('✅ Quill editor initialized successfully');
                resolve(quillInstance);
            }, 100); // Small delay to ensure DOM is ready

        } catch (error) {
            isInitializing = false;
            console.error('❌ Error initializing Quill:', error);
            reject(error);
        }
    });
};

/**
 * Get Quill content as HTML
 * Returns empty string if editor not initialized
 */
window.getQuillContent = function() {
    if (!quillInstance) {
        console.error('❌ Quill not initialized - cannot get content');
        return '';
    }

    try {
        const html = quillInstance.root.innerHTML;
        console.log('📄 Got Quill content:', html.substring(0, 50) + '...');
        return html;
    } catch (error) {
        console.error('❌ Error getting Quill content:', error);
        return '';
    }
};

/**
 * Set Quill content with proper initialization check
 * Waits for editor to be ready before setting content
 */
window.setQuillContent = async function(html) {
    console.log('🔄 Setting Quill content...');

    try {
        // Wait for Quill to be initialized
        if (!quillInstance) {
            console.log('⏳ Quill not ready, initializing first...');
            await window.initQuillEditor();
        }

        // Additional safety check
        if (!quillInstance) {
            console.error('❌ Quill failed to initialize');
            return false;
        }

        // Set content
        if (html && html.trim() !== '' && html !== '<p><br></p>') {
            quillInstance.root.innerHTML = html;
            console.log('✅ Quill content set successfully');
        } else {
            quillInstance.setText('');
            console.log('✅ Quill content cleared');
        }

        return true;
    } catch (error) {
        console.error('❌ Error setting Quill content:', error);
        return false;
    }
};

/**
 * Clear Quill content
 */
window.clearQuillContent = function() {
    if (!quillInstance) {
        console.error('❌ Quill not initialized - cannot clear content');
        return;
    }

    try {
        quillInstance.setText('');
        console.log('✅ Quill content cleared');
    } catch (error) {
        console.error('❌ Error clearing Quill content:', error);
    }
};

/**
 * Check if Quill is initialized
 */
window.isQuillReady = function() {
    return quillInstance !== null && !isInitializing;
};

/**
 * Destroy Quill instance (for cleanup)
 */
window.destroyQuill = function() {
    if (quillInstance) {
        quillInstance = null;
        console.log('🗑️ Quill instance destroyed');
    }
};