/*
 @licstart  The following is the entire license notice for the JavaScript code in this file.

 The MIT License (MIT)

 Copyright (C) 1997-2020 by Dimitri van Heesch

 Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 and associated documentation files (the "Software"), to deal in the Software without restriction,
 including without limitation the rights to use, copy, modify, merge, publish, distribute,
 sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in all copies or
 substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 @licend  The above is the entire license notice for the JavaScript code in this file
*/
var NAVTREE =
[
  [ "Lexenda Abogados", "index.html", [
    [ "⚖️Lexenda - Sistema de Gestión de Despacho de Abogados", "md__r_e_a_d_m_e.html", [
      [ "📋 Descripción del Proyecto", "md__r_e_a_d_m_e.html#autotoc_md7", [
        [ "🎯 Características principales", "md__r_e_a_d_m_e.html#autotoc_md8", null ]
      ] ],
      [ "🛠️ Tecnologías Utilizadas", "md__r_e_a_d_m_e.html#autotoc_md10", [
        [ "<b>Framework y Lenguajes</b>", "md__r_e_a_d_m_e.html#autotoc_md11", null ],
        [ "<b>Arquitectura y Patrones</b>", "md__r_e_a_d_m_e.html#autotoc_md12", null ],
        [ "<b>Acceso a Datos</b>", "md__r_e_a_d_m_e.html#autotoc_md13", null ],
        [ "<b>Generación de Informes</b>", "md__r_e_a_d_m_e.html#autotoc_md14", null ],
        [ "<b>Pruebas</b>", "md__r_e_a_d_m_e.html#autotoc_md15", null ],
        [ "<b>Control de versiones</b>", "md__r_e_a_d_m_e.html#autotoc_md16", null ]
      ] ],
      [ "📦 Instalación", "md__r_e_a_d_m_e.html#autotoc_md18", [
        [ "<b>Requisitos previos</b>", "md__r_e_a_d_m_e.html#autotoc_md19", null ],
        [ "<b>Opción 1: Instalación mediante ClickOnce (Recomendada para usuarios finales)</b>", "md__r_e_a_d_m_e.html#autotoc_md21", [
          [ "<b>Pasos para el administrador del sistema:</b>", "md__r_e_a_d_m_e.html#autotoc_md22", null ],
          [ "<b>Pasos para el usuario final:</b>", "md__r_e_a_d_m_e.html#autotoc_md23", null ]
        ] ],
        [ "<b>Opción 2: Instalación manual (Para desarrolladores)</b>", "md__r_e_a_d_m_e.html#autotoc_md25", [
          [ "<b>1. Clonar el repositorio</b>", "md__r_e_a_d_m_e.html#autotoc_md26", null ],
          [ "<b>2. Configurar la base de datos</b>", "md__r_e_a_d_m_e.html#autotoc_md27", null ],
          [ "<b>3. Restaurar paquetes NuGet</b>", "md__r_e_a_d_m_e.html#autotoc_md28", null ],
          [ "<b>4. Compilar la solución</b>", "md__r_e_a_d_m_e.html#autotoc_md29", null ],
          [ "<b>5. Ejecutar la aplicación</b>", "md__r_e_a_d_m_e.html#autotoc_md30", null ]
        ] ]
      ] ],
      [ "🚀 Cómo Ejecutar", "md__r_e_a_d_m_e.html#autotoc_md32", [
        [ "<b>Ejecución normal</b>", "md__r_e_a_d_m_e.html#autotoc_md33", null ]
      ] ],
      [ "📸 Capturas de Pantalla", "md__r_e_a_d_m_e.html#autotoc_md35", [
        [ "<b>1. Ventana Principal - Gestión de Clientes</b>", "md__r_e_a_d_m_e.html#autotoc_md36", null ],
        [ "<b>2. Gestión de Expedientes</b>", "md__r_e_a_d_m_e.html#autotoc_md38", null ],
        [ "<b>3. Gestión de Actuaciones</b>", "md__r_e_a_d_m_e.html#autotoc_md40", null ],
        [ "<b>4. Gestión de Actuaciones</b>", "md__r_e_a_d_m_e.html#autotoc_md42", null ],
        [ "<b>5. Selección de Informes</b>", "md__r_e_a_d_m_e.html#autotoc_md44", null ]
      ] ],
      [ "🧪 Pruebas Unitarias y de Integración", "md__r_e_a_d_m_e.html#autotoc_md46", [
        [ "<b>Ejecutar todas las pruebas</b>", "md__r_e_a_d_m_e.html#autotoc_md47", null ],
        [ "<b>Pruebas incluidas</b>", "md__r_e_a_d_m_e.html#autotoc_md48", [
          [ "<b>1. ClienteEmailTests</b> - Validación de formato de emails", "md__r_e_a_d_m_e.html#autotoc_md49", null ],
          [ "<b>2. ClienteTelefonoTests</b> - Validación de teléfonos españoles", "md__r_e_a_d_m_e.html#autotoc_md50", null ],
          [ "<b>3. CitaTests</b> - Control de conflictos horarios. Prueba de integración que verifica la restricción de citas duplicadas", "md__r_e_a_d_m_e.html#autotoc_md51", null ]
        ] ]
      ] ],
      [ "🔒 Seguridad y Validaciones", "md__r_e_a_d_m_e.html#autotoc_md53", [
        [ "<b>Validaciones implementadas</b>", "md__r_e_a_d_m_e.html#autotoc_md54", null ],
        [ "<b>Restricciones de base de datos</b>", "md__r_e_a_d_m_e.html#autotoc_md55", null ]
      ] ],
      [ "🤝 Contribuir", "md__r_e_a_d_m_e.html#autotoc_md57", [
        [ "<b>Guías para contribuir</b>", "md__r_e_a_d_m_e.html#autotoc_md58", null ]
      ] ],
      [ "👨‍💻 Autores", "md__r_e_a_d_m_e.html#autotoc_md60", [
        [ "<b>Desarrollador Principal</b>", "md__r_e_a_d_m_e.html#autotoc_md61", null ]
      ] ],
      [ "🌟 ¡Dale una estrella!", "md__r_e_a_d_m_e.html#autotoc_md63", null ]
    ] ],
    [ "Namespaces", "namespaces.html", [
      [ "Namespace List", "namespaces.html", "namespaces_dup" ]
    ] ],
    [ "Classes", "annotated.html", [
      [ "Class List", "annotated.html", "annotated_dup" ],
      [ "Class Index", "classes.html", null ],
      [ "Class Hierarchy", "hierarchy.html", "hierarchy" ],
      [ "Class Members", "functions.html", [
        [ "All", "functions.html", null ],
        [ "Functions", "functions_func.html", null ],
        [ "Properties", "functions_prop.html", null ],
        [ "Events", "functions_evnt.html", null ]
      ] ]
    ] ]
  ] ]
];

var NAVTREEINDEX =
[
"annotated.html",
"class_sistema_gestion_despacho_1_1_view_1_1_views_1_1_confirm_dialog.html#a51ab92836fa1f4142510c0543eac53c1"
];

var SYNCONMSG = 'click to disable panel synchronization';
var SYNCOFFMSG = 'click to enable panel synchronization';
var LISTOFALLMEMBERS = 'List of all members';