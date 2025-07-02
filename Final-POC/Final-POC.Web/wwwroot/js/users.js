$(document).ready(function () {
    // Initialize common functionality for the application
    Common.init();

    // Configure toastr notification settings
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

    // Initialize user management functionality
    UserManagement.init();
});

// UserManagement object containing all user-related functionality
var UserManagement = {
    // Initialize user management features
    init: function () {
        this.bindEvents();
        // Fetch all users if the user list is empty
        if ($(".alerts-content-single").length === 0) {
            this.fetchAllUsers();
        }
    },

    // Bind event handlers to DOM elements
    bindEvents: function () {
        // Handle add user form submission
        $("#formAddUser").off("submit").on("submit", function (e) {
            e.preventDefault();
            UserManagement.createUser();
        });

        // Handle edit user form submission
        $("#formEditUser").off("submit").on("submit", function (e) {
            e.preventDefault();
            UserManagement.updateUser();
        });

        // Handle view user details click (delegated event)
        $(".alerts-content").off("click", ".view-user").on("click", ".view-user", function () {
            const userId = $(this).data("id");
            UserManagement.viewUser(userId);
        });

        // Handle edit user click (delegated event)
        $(".alerts-content").off("click", ".edit-user").on("click", ".edit-user", function () {
            const userId = $(this).data("id");
            UserManagement.loadUserForEdit(userId);
        });

        // Handle delete user click (delegated event)
        $(".alerts-content").off("click", ".delete-user").on("click", ".delete-user", function () {
            const userId = $(this).data("id");
            UserManagement.deleteUser(userId);
        });

        // Handle apply filter button click
        $("#btnApplyFilter").off("click").on("click", function () {
            UserManagement.filterUsers();
        });

        // Handle reset filter button click
        $("#btnResetFilter").off("click").on("click", function () {
            UserManagement.resetFilter();
        });
    },

    // Create a new user
    createUser: function () {
        // Show loading indicator
        Common.showLoader();

        // Prepare user data for creation
        const registerUserDto = {
            username: $("#txtUsername").val(),
            email: $("#txtEmail").val(),
            password: $("#txtPassword").val(),
            role: $("#txtRole").val()
        };

        // Send AJAX request to create user
        $.ajax({
            url: "/User/Create",
            type: "POST",
            data: registerUserDto,
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    // Clear form fields
                    $("#txtUsername").val("");
                    $("#txtEmail").val("");
                    $("#txtPassword").val("");
                    Common.closeDrawer("#drawerAddUser");
                    // Refresh user list
                    UserManagement.fetchAllUsers();
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr) {
                const message = xhr.responseJSON?.message || "Failed to create user. Please try again.";
                toastr.error(message);
            },
            complete: function () {
                // Hide loading indicator
                Common.hideLoader();
            }
        });
    },

    // Load user data for editing
    loadUserForEdit: function (userId) {
        // Show loading indicator
        Common.showLoader();

        // Send AJAX request to get user data
        $.ajax({
            url: "/User/Edit/" + userId,
            type: "GET",
            success: function (response) {
                if (response.success) {
                    const user = response.data;

                    // Populate edit form fields
                    $("#hdnUserId").val(user.id);
                    $("#txtEditUsername").val(user.username);
                    $("#txtEditEmail").val(user.email);

                    // Open edit drawer
                    Common.openDrawer("#drawerEditUser");
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr) {
                const message = xhr.responseJSON?.message || "Failed to load user details. Please try again.";
                toastr.error(message);
            },
            complete: function () {
                // Hide loading indicator
                Common.hideLoader();
            }
        });
    },

    // Update an existing user
    updateUser: function () {
        // Show loading indicator
        Common.showLoader();

        // Prepare user data for update
        const updateUserDto = {
            id: $("#hdnUserId").val(),
            username: $("#txtEditUsername").val(),
            email: $("#txtEditEmail").val()
        };

        // Send AJAX request to update user
        $.ajax({
            url: "/User/Edit",
            type: "PUT",
            data: updateUserDto,
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    Common.closeDrawer("#drawerEditUser");
                    // Refresh user list
                    UserManagement.fetchAllUsers();
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr) {
                const message = xhr.responseJSON?.message || "Failed to update user. Please try again.";
                toastr.error(message);
            },
            complete: function () {
                // Hide loading indicator
                Common.hideLoader();
            }
        });
    },

    // View user details
    viewUser: function (userId) {
        // Show loading indicator
        Common.showLoader();

        // Send AJAX request to get user details
        $.ajax({
            url: "/User/Details/" + userId,
            type: "GET",
            success: function (response) {
                if (response.success) {
                    const user = response.data;

                    // Populate view drawer fields
                    $("#viewUserUsername").text(user.username);
                    $("#viewUsername").text(user.username);
                    $("#viewEmail").text(user.email);
                    $("#viewRole").text(user.role);

                    // Open view drawer
                    Common.openDrawer("#drawerViewUser");
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr) {
                const message = xhr.responseJSON?.message || "Failed to load user details. Please try again.";
                toastr.error(message);
            },
            complete: function () {
                // Hide loading indicator
                Common.hideLoader();
            }
        });
    },

    // Delete a user
    deleteUser: function (userId) {
        // Confirm deletion with user
        if (!confirm("Are you sure you want to delete this user?")) {
            return;
        }

        // Show loading indicator
        Common.showLoader();

        // Send AJAX request to delete user
        $.ajax({
            url: "/User/Delete/" + userId,
            type: "DELETE",
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    // Refresh user list
                    UserManagement.fetchAllUsers();
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr) {
                const message = xhr.responseJSON?.message || "Failed to delete user. Please try again.";
                toastr.error(message);
            },
            complete: function () {
                // Hide loading indicator
                Common.hideLoader();
            }
        });
    },

    // Filter users based on criteria
    filterUsers: function () {
        // Show loading indicator
        Common.showLoader();

        // Get filter criteria
        const username = $("#txtFilterUsername").val();
        const email = $("#txtFilterEmail").val();

        // Send AJAX request to filter users
        $.ajax({
            url: "/User/Search",
            type: "POST",
            data: {
                username: username,
                email: email
            },
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    // Clear existing user list
                    $(".alerts-content").empty();

                    // Update user count
                    $(".alerts-title h1").text("Users (" + response.data.length + ")");

                    // Add filtered users to list
                    response.data.forEach(function (user) {
                        const userElement = UserManagement.createUserElement(user);
                        $(".alerts-content").append(userElement);
                    });

                    // Rebind events for new elements
                    UserManagement.bindEvents();

                    // Close filter panel
                    Common.hideFilter('#divFilters');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr) {
                const message = xhr.responseJSON?.message || "Failed to filter users. Please try again.";
                toastr.error(message);
            },
            complete: function () {
                // Hide loading indicator
                Common.hideLoader();
            }
        });
    },

    // Reset filter criteria and refresh user list
    resetFilter: function () {
        // Clear filter fields
        $("#txtFilterUsername").val("");
        $("#txtFilterEmail").val("");

        // Refresh user list
        UserManagement.fetchAllUsers();
        // Close filter panel
        Common.hideFilter('#divFilters');
    },

    // Fetch all users
    fetchAllUsers: function () {
        // Show loading indicator
        Common.showLoader();

        // Send AJAX request to get all users
        $.ajax({
            url: "/User/GetAllUsers",
            type: "GET",
            success: function (response) {
                if (response.success) {
                    // Clear existing user list
                    $(".alerts-content").empty();

                    // Update user count
                    $(".alerts-title h1").text("Users (" + response.data.length + ")");

                    // Add users to list
                    response.data.forEach(function (user) {
                        const userElement = UserManagement.createUserElement(user);
                        $(".alerts-content").append(userElement);
                    });
                } else {
                    toastr.error(response.message || "Failed to fetch users");
                }
            },
            error: function (xhr) {
                const message = xhr.responseJSON?.message || "Failed to fetch users. Please try again.";
                toastr.error(message);
            },
            complete: function () {
                // Hide loading indicator
                Common.hideLoader();
            }
        });
    },

    // Create HTML element for a user
    createUserElement: function (user) {
        if (user.id != 1) {
            return `
            <div class="alerts-content-single" data-id="${user.id}">
                <div class="alerts-content-inner">
                    <div class="content-main">
                        <div class="content-title">
                            <span>${user.username}</span>
                        </div>
                        <div class="content-desc">
                        </div>
                    </div>
                </div>

                <div class="alerts-content-inner">
                    <div class="content-main">
                        <div class="content-desc">
                            ${user.email}
                        </div>
                    </div>
                </div>

                <div class="alerts-content-inner">
                    <div class="content-main">
                        <div class="content-last">
                            <span class="closed btn-all-case">
                                <i class="nt-tick"></i>
                                <span class="alert-text">
                                    ${user.role}
                                </span>
                            </span>

                            <div class="edit-links">
                                <em class="nt-more"></em>
                                <div class="link-box">
                                    <div class="link view-user" data-id="${user.id}">
                                        <em class="nt-eye"></em>
                                        View
                                    </div>
                                    <div class="link edit-user" data-id="${user.id}">
                                        <em class="nt-edit"></em>
                                        Edit
                                    </div>
                                    <div class="link delete-user" data-id="${user.id}">
                                        <em class="nt-bin"></em>
                                        Delete
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            `;
        }
        
    }
};