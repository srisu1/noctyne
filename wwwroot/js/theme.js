// theme.js - Global Theme Management
// Place in: wwwroot/js/theme.js

window.themeManager = {
    // Set theme (light or dark)
    setTheme: function(theme) {
        console.log('🎨 Setting theme:', theme);
        
        if (theme === 'dark') {
            document.body.classList.add('dark-theme');
            document.documentElement.classList.add('dark-theme');
        } else {
            document.body.classList.remove('dark-theme');
            document.documentElement.classList.remove('dark-theme');
        }
        
        // Store in localStorage for persistence
        localStorage.setItem('moodjournal-theme', theme);
        
        return true;
    },
    
    // Get current theme
    getTheme: function() {
        return document.body.classList.contains('dark-theme') ? 'dark' : 'light';
    },
    
    // Toggle theme
    toggleTheme: function() {
        var current = this.getTheme();
        var newTheme = current === 'dark' ? 'light' : 'dark';
        this.setTheme(newTheme);
        return newTheme;
    },
    
    // Initialize theme from localStorage
    initTheme: function() {
        var saved = localStorage.getItem('moodjournal-theme');
        if (saved) {
            this.setTheme(saved);
        }
    }
};

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    window.themeManager.initTheme();
});
