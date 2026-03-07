/**
 * SuVac - Internacionalización (I18N)
 * Librería: i18next (https://www.i18next.com/)
 * Persistencia: localStorage (clave: "suvac_lang")
 * Idiomas soportados: es (Español), en (English)
 *
 * Uso en vistas: agregar data-i18n="namespace.clave" a cualquier elemento.
 * Ejemplo: <h1 data-i18n="home.bienvenido"></h1>
 *
 * Para placeholders:  data-i18n="[placeholder]ganado.nombre"
 * Para title/aria:    data-i18n="[title]comun.editar"
 */

(function () {
    'use strict';

    // ─── Constantes ──────────────────────────────────────────────────────────
    const STORAGE_KEY = 'suvac_lang';
    const DEFAULT_LANG = 'es';
    const SUPPORTED = ['es', 'en'];

    // ─── Detectar idioma guardado o usar español por defecto ─────────────────
    function getStoredLang() {
        try {
            const stored = localStorage.getItem(STORAGE_KEY);
            return SUPPORTED.includes(stored) ? stored : DEFAULT_LANG;
        } catch {
            return DEFAULT_LANG;
        }
    }

    // ─── Guardar idioma en localStorage ─────────────────────────────────────
    function storeLang(lang) {
        try {
            localStorage.setItem(STORAGE_KEY, lang);
        } catch { }
    }

    async function loadTranslation(lang) {
        const url = `/js/i18n/${lang}.json?v=1`;
        const resp = await fetch(url);
        if (!resp.ok) throw new Error(`No se pudo cargar: ${url}`);
        return resp.json();
    }

    
    function resolveKey(obj, key) {
        return key.split('.').reduce((o, k) => (o && o[k] !== undefined ? o[k] : null), obj);
    }

    function applyTranslations(translations) {
        document.querySelectorAll('[data-i18n]').forEach(el => {
            const raw = el.getAttribute('data-i18n');
           
            raw.split(';').forEach(part => {
                part = part.trim();
                if (!part) return;

                const attrMatch = part.match(/^\[([^\]]+)\](.+)$/);
                if (attrMatch) {
              
                    const attr = attrMatch[1];
                    const key = attrMatch[2];
                    const value = resolveKey(translations, key);
                    if (value !== null) el.setAttribute(attr, value);
                } else {
                    
                    const value = resolveKey(translations, part);
                    if (value === null) return;

                    if (el.children.length === 0) {
                     
                        el.textContent = value;
                    } else {
                       
                        let textSpan = el.querySelector(':scope > .i18n-text');
                        if (!textSpan) {
                            textSpan = document.createElement('span');
                            textSpan.className = 'i18n-text';
                          
                            Array.from(el.childNodes)
                                .filter(n => n.nodeType === Node.TEXT_NODE)
                                .forEach(n => { n.textContent = ''; });
                            el.appendChild(textSpan);
                        }
                        textSpan.textContent = value;
                    }
                }
            });
        });

        // Actualizar el título de la página si existe data-i18n-title
        const titleKey = document.body.getAttribute('data-i18n-title');
        if (titleKey) {
            const value = resolveKey(translations, titleKey);
            if (value) document.title = `${value} - SuVac`;
        }
    }

   
    function updateLangButton(lang, translations) {
        const btn = document.getElementById('btn-lang-switch');
        if (!btn) return;

        const opposite = lang === 'es' ? 'en' : 'es';
        btn.setAttribute('data-lang-target', opposite);

   
        const label = resolveKey(translations, 'lang.switchBtn');
        if (label) {
            const textSpan = btn.querySelector('.lang-btn-text');
            if (textSpan) textSpan.textContent = label;
        }

        // Indicador del idioma activo
        const badge = document.getElementById('lang-current-badge');
        if (badge) {
            const currentLabel = resolveKey(translations, 'lang.currentLang');
            if (currentLabel) badge.textContent = currentLabel;
        }
    }

    //  FuncióN Cambiar 
    async function changeLanguage(lang) {
        if (!SUPPORTED.includes(lang)) lang = DEFAULT_LANG;

        try {
            const translations = await loadTranslation(lang);
            storeLang(lang);

            // Actualizar atributo lang del HTML
            document.documentElement.lang = lang === 'es' ? 'es' : 'en';

            applyTranslations(translations);
            updateLangButton(lang, translations);

            document.dispatchEvent(new CustomEvent('suvac:langChanged', { detail: { lang, translations } }));
        } catch (err) {
            console.error('[SuVac i18n] Error al cargar traducciones:', err);
        }
    }

    // ─── Inicialización al cargar el DOM 
    document.addEventListener('DOMContentLoaded', async function () {
        const lang = getStoredLang();
        await changeLanguage(lang);

        document.addEventListener('click', function (e) {
            const btn = e.target.closest('#btn-lang-switch');
            if (btn) {
                const target = btn.getAttribute('data-lang-target') || (getStoredLang() === 'es' ? 'en' : 'es');
                changeLanguage(target);
            }
        });
    });

  
    window.SuVacI18n = {
        change: changeLanguage,
        current: getStoredLang
    };

})();
