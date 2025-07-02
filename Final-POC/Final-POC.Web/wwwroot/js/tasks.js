$(document).ready(function () {
    // Initialize task management
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    Tasks.init();
});

/**
 * Tasks module for task management functionality
 */
const Tasks = (function () {
    // Public API
    const publicAPI = {
        init: initialize
    };

    // Cache DOM elements
    const cache = {
        // Forms and drawers
        addTaskForm: $('#formAddTask'),
        editTaskForm: $('#formEditTask'),
        updateStatusForm: $('#formUpdateStatus'),
        filterForm: $('#formFilterTasks'),
        sortForm: $('#divSorting'),

        // Buttons
        saveTaskBtn: $('#btnSaveTask'),
        updateTaskBtn: $('#btnUpdateTask'),
        applyFilterBtn: $('#btnApplyFilter'),
        resetFilterBtn: $('#btnResetFilter'),
        applySortBtn: $('#btnApplySort'),
        resetSortBtn: $('#btnResetSort'),

        // Drawers
        addTaskDrawer: $('#drawerAddTask'),
        editTaskDrawer: $('#drawerEditTask'),
        viewTaskDrawer: $('#drawerViewTask'),
        updateStatusDrawer: $('#drawerUpdateStatus'),

        // Content areas
        tasksContent: $('.alerts-content'),

        // Sort order elements
        sortAscending: $('#sortAscending'),
        sortDescending: $('#sortDescending')
    };

    // Current sort state
    const sortState = {
        field: 'CreatedAt', // Default sort field
        order: 'asc'       // Default sort order (matches view's active sortAscending)
    };

    // Current filter state
    const filterState = {
        title: null,
        status: null,
        assignedToId: null,
        createdAfter: null,
        createdBefore: null
    };

    // Event binding
    function bindEvents() {
        // Add task form submission
        cache.addTaskForm.on('submit', function (e) {
            e.preventDefault();
            createTask();
        });

        // Edit task form submission
        cache.editTaskForm.on('submit', function (e) {
            e.preventDefault();
            updateTask();
        });

        // Update status form submission
        cache.updateStatusForm.on("submit", function (e) {
            e.preventDefault();
            e.stopImmediatePropagation();
            updateTaskStatus();
        });

        // Filter actions
        cache.applyFilterBtn.on('click', function () {
            updateFilterState();
            fetchTasks();
        });

        cache.resetFilterBtn.on('click', resetFilter);

        // Sort actions
        cache.applySortBtn.on('click', fetchTasks); // Use fetch endpoint for sorting
        cache.resetSortBtn.on('click', resetSort);

        // Sort order toggle
        cache.sortAscending.on('click', function () {
            selectSortOrder('asc');
        });

        cache.sortDescending.on('click', function () {
            selectSortOrder('desc');
        });

        // Sort field selection
        $('input[name="sortListing"]').on('change', function () {
            sortState.field = $(this).val();
        });

        // Dynamic event binding for task actions
        $(document).on('click', '.view-task', function () {
            const taskId = $(this).data('id');
            viewTask(taskId);
        });

        $(document).on('click', '.edit-task', function () {
            const taskId = $(this).data('id');
            editTask(taskId);
        });

        $(document).on('click', '.update-status', function () {
            const taskId = $(this).data('id');
            const currentStatus = $(this).data('status');
            openStatusUpdateDrawer(taskId, currentStatus);
        });

        // Initialize user dropdown for task assignment only if user is admin
        if (isAdmin()) {
            loadUsersList();
        }
    }

    // Update filter state from form values
    function updateFilterState() {
        const createdAfter = $('#txtFilterCreatedAfter').val();
        const createdBefore = $('#txtFilterCreatedBefore').val();

        filterState.title = $('#txtFilterTitle').val().trim() || null;
        filterState.status = $('#ddlFilterStatus').val() || null;
        filterState.assignedToId = $('#ddlFilterAssignedTo').val() ? parseInt($('#ddlFilterAssignedTo').val()) : null;
        filterState.createdAfter = createdAfter ? new Date(createdAfter).toISOString() : null;
        filterState.createdBefore = createdBefore ? new Date(createdBefore).toISOString() : null;
    }

    // Fetch tasks based on current filter and sort state
    function fetchTasks() {
        Common.showLoader();

        // Different endpoint and behavior based on user type
        if (isAdmin()) {
            // Admin uses the filter endpoint with all filtering options
            fetchTasksForAdmin();
        } else {
            // Regular user just gets their tasks
            fetchTasksForUser();
        }
    }

    // Fetch tasks for admin users with filtering
    function fetchTasksForAdmin() {
        const filterData = {
            title: filterState.title,
            status: filterState.status,
            assignedToId: filterState.assignedToId,
            createdAfter: filterState.createdAfter,
            createdBefore: filterState.createdBefore,
            sortBy: sortState.field,
            sortDescending: sortState.order === 'desc'
        };

        // Add anti-forgery token
        const token = $('input[name="__RequestVerificationToken"]').val();
        console.log('Admin fetching tasks with criteria:', filterData);

        $.ajax({
            url: '/Task/Filter',
            type: 'POST',
            data: filterData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    Common.hideFilter('#divFilters');
                    Common.hideFilter('#divSorting');
                    updateTasksList(response.data);
                    updateTasksCount(response.data.length);
                } else {
                    toastr.error(response.message || 'Failed to fetch tasks');
                }
                Common.hideLoader();
            },
            error: function (xhr) {
                console.error('Error fetching tasks:', xhr.responseText);
                toastr.error('Failed to fetch tasks');
                Common.hideLoader();
            }
        });
    }

    // Fetch tasks for regular users (their assigned tasks only)
    function fetchTasksForUser() {
        $.ajax({
            url: '/Task/MyTasks',
            type: 'GET',
            success: function (response) {
                // Handle the view returned directly from controller
                if (response.success) {
                    updateTasksList(response.data);
                    updateTasksCount(response.data.length);
                }
                else {
                    toastr.error(response.message || 'Failed to fetch tasks');
                }
                Common.hideLoader();
            },
            error: function (xhr) {
                console.error('Error fetching tasks:', xhr.responseText);
                toastr.error('Failed to fetch tasks');
                Common.hideLoader();
            }
        });
    }

    // Set sorting order and update UI
    function selectSortOrder(order) {
        sortState.order = order;

        if (order === 'asc') {
            cache.sortAscending.addClass('active');
            cache.sortDescending.removeClass('active');
        } else {
            cache.sortAscending.removeClass('active');
            cache.sortDescending.addClass('active');
        }
    }

    // Load users list for assignment dropdown
    function loadUsersList() {
        // Only load users list if user is admin
        if (!isAdmin()) return;

        Common.showLoader();

        // Make API call to get users list
        $.ajax({
            url: '/User/GetAllUsers',
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    populateUserDropdowns(response.data);
                } else {
                    console.error('Failed to load users list:', response.message);
                    toastr.error('Failed to load users list');
                }
                Common.hideLoader();
            },
            error: function (xhr) {
                console.error('Error loading users list:', xhr.responseText);
                toastr.error('Failed to load users list');
                Common.hideLoader();
            }
        });
    }

    // Populate user dropdowns for assignment
    function populateUserDropdowns(users) {
        const dropdowns = [
            $('#ddlAssignedTo'),
            $('#ddlEditAssignedTo'),
            $('#ddlFilterAssignedTo')
        ];

        dropdowns.forEach(dropdown => {
            // Preserve the first option and clear the rest
            const firstOption = dropdown.find('option:first');
            dropdown.empty().append(firstOption);

            // Add user options
            users.forEach(user => {
                dropdown.append(`<option value="${user.id}">${user.username}</option>`);
            });
        });
    }

    // Create new task
    function createTask() {
        if (!validateForm(cache.addTaskForm)) return;

        Common.showLoader();

        const taskData = {
            title: $('#txtTitle').val().trim(),
            description: $('#txtDescription').val().trim(),
            createdById: parseInt($('#hdnCreatedById').val()),
            assignedToId: $('#ddlAssignedTo').val() ? parseInt($('#ddlAssignedTo').val()) : null
        };

        // Add anti-forgery token
        const token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: '/Task/Create',
            type: 'POST',
            data: taskData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success('Task created successfully');
                    Common.closeDrawer('#drawerAddTask');
                    clearForm(cache.addTaskForm);
                    // Refetch tasks instead of page reload
                    fetchTasks();
                } else {
                    toastr.error(response.message || 'Failed to create task');
                }
                Common.hideLoader();
            },
            error: function (xhr) {
                console.error('Error creating task:', xhr.responseText);
                if (xhr.responseJSON && xhr.responseJSON.errors) {
                    displayValidationErrors(xhr.responseJSON.errors);
                } else {
                    toastr.error('Failed to create task');
                }
                Common.hideLoader();
            }
        });
    }

    // Edit task
    function editTask(taskId) {
        Common.showLoader();

        $.ajax({
            url: `/Task/Details/${taskId}`,
            type: 'GET',
            success: function (response) {
                if (response.success && response.data) {
                    // Populate edit form
                    $('#hdnTaskId').val(response.data.id);
                    $('#txtEditTitle').val(response.data.title);
                    $('#txtEditDescription').val(response.data.description);
                    $('#ddlEditAssignedTo').val(response.data.assignedToId ? response.data.assignedToId : '');

                    // Open edit drawer
                    Common.openDrawer('#drawerEditTask');
                } else {
                    toastr.error(response.message || 'Failed to load task details');
                }
                Common.hideLoader();
            },
            error: function (xhr) {
                console.error('Error loading task details:', xhr.responseText);
                toastr.error('Failed to load task details');
                Common.hideLoader();
            }
        });
    }

    // Update task
    function updateTask() {
        if (!validateForm(cache.editTaskForm)) return;
        Common.showLoader();
        const taskId = parseInt($('#hdnTaskId').val());
        if (!taskId) {
            toastr.error('Invalid task ID');
            Common.hideLoader();
            return;
        }
        const taskData = {
            Id: taskId,
            Title: $('#txtEditTitle').val().trim(),
            Description: $('#txtEditDescription').val().trim(),
            AssignedToId: $('#ddlEditAssignedTo').val() ? parseInt($('#ddlEditAssignedTo').val()) : null
        };
        // Add anti-forgery token
        const token = $('input[name="__RequestVerificationToken"]').val();
        console.log(taskData);
        $.ajax({
            url: '/Task/Edit',
            type: 'PUT',
            data: taskData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success('Task updated successfully');
                    Common.closeDrawer('#drawerEditTask');
                    // Refetch tasks instead of page reload
                    fetchTasks();
                } else {
                    toastr.error(response.message || 'Failed to update task');
                }
                Common.hideLoader();
            },
            error: function (xhr) {
                console.error('Error updating task:', xhr.responseText);
                if (xhr.responseJSON && xhr.responseJSON.errors) {
                    displayValidationErrors(xhr.responseJSON.errors);
                } else {
                    toastr.error('Failed to update task');
                }
                Common.hideLoader();
            }
        });
    }

    // View task details
    function viewTask(taskId) {
        Common.showLoader();

        $.ajax({
            url: `/Task/Details/${taskId}`,
            type: 'GET',
            success: function (response) {
                if (response.success && response.data) {
                    const task = response.data;

                    // Populate view drawer
                    $('#viewTaskTitle').text(task.title);
                    $('#viewTitle').text(task.title);
                    const displayStatus = formatStatusForDisplay(task.status);
                    $('#viewStatus').text(displayStatus);
                    $('#viewCreatedBy').text(task.createdByName || 'N/A');
                    $('#viewCreatedAt').text(formatDate(task.createdAt));
                    $('#viewAssignedTo').text(task.assignedToName || 'Unassigned');
                    $('#viewDescription').text(task.description || 'No description provided');

                    // Open view drawer
                    Common.openDrawer('#drawerViewTask');
                } else {
                    toastr.error(response.message || 'Failed to load task details');
                }
                Common.hideLoader();
            },
            error: function (xhr) {
                console.error('Error loading task details:', xhr.responseText);
                toastr.error('Failed to load task details');
                Common.hideLoader();
            }
        });
    }

    // Open status update drawer
    function openStatusUpdateDrawer(taskId, currentStatus) {
        Common.showLoader();

        $.ajax({
            url: `/Task/Details/${taskId}`,
            type: 'GET',
            success: function (response) {
                if (response.success && response.data) {
                    const task = response.data;

                    // Set task ID and title for status update
                    $('#hdnStatusTaskId').val(task.id);
                    $('#updateStatusTaskTitle').text(task.title);

                    // Set the current status
                    $(`input[name="taskStatus"][value="${task.status}"]`).prop('checked', true);

                    // Open status update drawer
                    Common.openDrawer('#drawerUpdateStatus');
                } else {
                    toastr.error(response.message || 'Failed to load task details');
                }
                Common.hideLoader();
            },
            error: function (xhr) {
                console.error('Error loading task details:', xhr.responseText);
                toastr.error('Failed to load task details');
                Common.hideLoader();
            }
        });
    }

    // Update task status
    function updateTaskStatus() {
        const taskId = parseInt($('#hdnStatusTaskId').val());
        const newStatus = $('input[name="taskStatus"]:checked').val();

        if (!taskId || !newStatus) {
            toastr.error('Invalid task status update data');
            return;
        }

        Common.showLoader();

        const statusData = {
            id: taskId,
            status: newStatus
        };

        // Add anti-forgery token
        const token = $('input[name="__RequestVerificationToken"]').val();
        console.log(statusData);

        $.ajax({
            url: '/Task/UpdateStatus',
            type: 'PATCH',
            data: statusData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success('Task status updated successfully');
                    Common.closeDrawer('#drawerUpdateStatus');
                    // Refetch tasks instead of page reload
                    fetchTasks();
                } else {
                    toastr.error(response.message || 'Failed to update task status');
                }
                Common.hideLoader();
            },
            error: function (xhr) {
                console.error('Error updating task status:', xhr.responseText);
                toastr.error('Failed to update task status');
                Common.hideLoader();
            }
        });
    }

    // Apply filter and sort
    function applyFilter() {
        updateFilterState();
        fetchTasks();
    }

    // Reset filter
    function resetFilter() {
        $('#txtFilterTitle').val('');
        $('#ddlFilterStatus').val('');
        $('#ddlFilterAssignedTo').val('');
        $('#txtFilterCreatedAfter').val('');
        $('#txtFilterCreatedBefore').val('');

        // Reset filter state
        filterState.title = null;
        filterState.status = null;
        filterState.assignedToId = null;
        filterState.createdAfter = null;
        filterState.createdBefore = null;

        resetSort();
    }

    // Reset sort to default
    function resetSort() {
        $('input[name="sortListing"][value="CreatedAt"]').prop('checked', true);
        sortState.field = 'CreatedAt';
        selectSortOrder('asc');
        fetchTasks();
    }

    // Update tasks list with filtered/sorted data
    function updateTasksList(tasks) {
        cache.tasksContent.empty();

        if (tasks && tasks.length > 0) {
            tasks.forEach(task => {
                const statusClass = getStatusClass(task.status);
                const statusIcon = task.status === 'Done' ? 'nt-tick' : 'nt-clock';
                const displayStatus = formatStatusForDisplay(task.status);

                const taskHtml = `
                    <div class="alerts-content-single" data-id="${task.id}">
                        <div class="alerts-content-inner">
                            <div class="content-main">
                                <div class="content-title">
                                    <span>${task.title}</span>
                                </div>
                                <div class="content-desc">
                                    ${task.assignedToName ? '<small>Assigned to: ' + task.assignedToName + '</small>' : '<small>Unassigned</small>'}
                                </div>
                            </div>
                        </div>
                        <div class="alerts-content-inner">
                            <div class="content-main">
                                <div class="content-desc">
                                    Created by: ${task.createdByName || 'N/A'}<br/>
                                    ${formatDate(task.createdAt)}
                                </div>
                            </div>
                        </div>
                        <div class="alerts-content-inner">
                            <div class="content-main">
                                <div class="content-last">
                                    <span class="${statusClass} btn-all-case">
                                        <i class="${statusIcon}"></i>
                                        <span class="alert-text">
                                            ${displayStatus}
                                        </span>
                                    </span>
                                    <div class="edit-links">
                                        <em class="nt-more"></em>
                                        <div class="link-box">
                                            <div class="link view-task" data-id="${task.id}">
                                                <em class="nt-eye"></em>
                                                View
                                            </div>
                                            ${isUserAllowedToUpdateStatus(task) ?
                        `<div class="link update-status" data-id="${task.id}" data-status="${task.status}">
                                                    <em class="nt-edit"></em>
                                                    Update Status
                                                </div>` : ''}
                                            ${isAdmin() ?
                        `<div class="link edit-task" data-id="${task.id}">
                                                    <em class="nt-edit"></em>
                                                    Edit
                                                </div>` : ''}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                `;

                cache.tasksContent.append(taskHtml);
            });
        } else {
            cache.tasksContent.html('<div class="no-records">No tasks found.</div>');
        }
    }

    // Get status class based on status value
    function getStatusClass(status) {
        switch (status) {
            case 'Done':
                return 'closed';
            case 'InProgress':
                return 'in-progress';
            default:
                return 'pending';
        }
    }

    // Format status for display
    function formatStatusForDisplay(status) {
        switch (status) {
            case 'InProgress':
                return 'In Progress';
            default:
                return status;
        }
    }

    // Update tasks count in the title
    function updateTasksCount(count) {
        $('.alerts-title h1').text(`Tasks (${count})`);
    }

    // Format date for display
    function formatDate(dateString) {
        const date = new Date(dateString);
        const options = { month: 'short', day: 'numeric', year: 'numeric' };
        return date.toLocaleDateString('en-US', options);
    }

    // Check if current user is admin
    function isAdmin() {
        return $('a[onclick="Common.openDrawer(\'#drawerAddTask\')"]').length > 0;
    }

    // Check if user is allowed to update task status
    function isUserAllowedToUpdateStatus(task) {
        const currentUserId = $('#hdnCreatedById').val();
        return isAdmin() || (task.assignedToId && task.assignedToId.toString() === currentUserId);
    }

    // Validate form
    function validateForm(form) {
        let isValid = true;

        form.find('[required]').each(function () {
            if (!$(this).val().trim()) {
                isValid = false;
                $(this).addClass('is-invalid');
            } else {
                $(this).removeClass('is-invalid');
            }
        });

        if (!isValid) {
            toastr.error('Please fill in all required fields');
        }

        return isValid;
    }

    // Clear form fields
    function clearForm(form) {
        form.find('input:not([type=hidden]), textarea, select').val('');
    }

    // Display validation errors
    function displayValidationErrors(errors) {
        if (Array.isArray(errors)) {
            let errorMessage = '<ul>';
            errors.forEach(error => {
                errorMessage += `<li>${error}</li>`;
            });
            errorMessage += '</ul>';
            toastr.error(errorMessage, 'Validation Error', {
                timeOut: 5000,
                extendedTimeOut: 2000
            });
        } else {
            toastr.error('Validation failed', 'Validation Error');
        }
    }

    // Initialize module
    function initialize() {
        bindEvents();
        // Initial task fetch with appropriate endpoint
        fetchTasks();
    }

    // Return public API
    return publicAPI;
})();