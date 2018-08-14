using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class AssignPersonTypeTowPage : ContentPage
    {
        void HandleAction()
        {
            Console.WriteLine("完成按钮");

        }


        void Handle_ItemSelected_1(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            SubsidiaryDepartment subsidiaryDepartment = e.SelectedItem as SubsidiaryDepartment;
            listV3.ItemsSource = subsidiaryDepartment.personnels;
            listV3.SelectedItem = subsidiaryDepartment.personnels[0];

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            Department department = e.SelectedItem as Department;
            listV2.ItemsSource = department.subsidiaryDepartments;
            listV2.SelectedItem = department.subsidiaryDepartments[0];
            SubsidiaryDepartment subsidiaryDepartment = listV2.SelectedItem as SubsidiaryDepartment;
            listV3.ItemsSource = subsidiaryDepartment.personnels;
            listV3.SelectedItem = subsidiaryDepartment.personnels[0];

        }

        private ObservableCollection<Department> departments = new ObservableCollection<Department>();

        public AssignPersonTypeTowPage()
        {
            InitializeComponent();

            departments.Add(new Department
            {
                name = "监测大队",
                subsidiaryDepartments = new ObservableCollection<SubsidiaryDepartment>
                {
                    new SubsidiaryDepartment{
                        name = "一中队",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二",
                            },
                            new Personnel {
                                name = "张三",
                            },
                            new Personnel {
                                name = "李四",
                            },
                        }
                    },

                    new SubsidiaryDepartment{
                        name = "2中队",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel {
                                name = "李四",
                            },
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二麻子",
                            },
                            new Personnel {
                                name = "张三",
                            },
                           
                        }
                    },

                    new SubsidiaryDepartment{
                        name = "3中队",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel {
                                name = "张三",
                            },
                            new Personnel {
                                name = "李四",
                            },
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二麻子",
                            },
                        }
                    },

                }
            });


            departments.Add(new Department
            {
                name = "环保局",
                subsidiaryDepartments = new ObservableCollection<SubsidiaryDepartment>
                {
                    new SubsidiaryDepartment{
                        name = "1局",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二",
                            },
                            new Personnel {
                                name = "张三",
                            },
                            new Personnel {
                                name = "李四",
                            },
                        }
                    },

                    new SubsidiaryDepartment{
                        name = "2局",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel {
                                name = "李四",
                            },
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二麻子",
                            },
                            new Personnel {
                                name = "张三",
                            },

                        }
                    },

                    new SubsidiaryDepartment{
                        name = "3局",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel {
                                name = "张三",
                            },
                            new Personnel {
                                name = "李四",
                            },
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二麻子",
                            },
                        }
                    },

                }
            });

            departments.Add(new Department
            {
                name = "监测大队231",
                subsidiaryDepartments = new ObservableCollection<SubsidiaryDepartment>
                {
                    new SubsidiaryDepartment{
                        name = "一中队",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二",
                            },
                            new Personnel {
                                name = "张三",
                            },
                            new Personnel {
                                name = "李四",
                            },
                        }
                    },

                    new SubsidiaryDepartment{
                        name = "2中队1234",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel {
                                name = "李四",
                            },
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二麻子",
                            },
                            new Personnel {
                                name = "张三",
                            },

                        }
                    },

                    new SubsidiaryDepartment{
                        name = "3中队",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel {
                                name = "张三",
                            },
                            new Personnel {
                                name = "李四",
                            },
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二麻子212",
                            },
                        }
                    },

                }
            });


            departments.Add(new Department
            {
                name = "环保局",
                subsidiaryDepartments = new ObservableCollection<SubsidiaryDepartment>
                {
                    new SubsidiaryDepartment{
                        name = "1局",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二",
                            },
                            new Personnel {
                                name = "张三",
                            },
                            new Personnel {
                                name = "李四",
                            },
                        }
                    },

                    new SubsidiaryDepartment{
                        name = "2局",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel {
                                name = "李四",
                            },
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二麻子12",
                            },
                            new Personnel {
                                name = "张三",
                            },

                        }
                    },

                    new SubsidiaryDepartment{
                        name = "3局",
                        personnels = new ObservableCollection<Personnel>{
                            new Personnel {
                                name = "张三",
                            },
                            new Personnel {
                                name = "李四",
                            },
                            new Personnel{
                                name = "陈大",
                            },
                            new Personnel {
                                name = "王二麻子",
                            },
                        }
                    },

                }
            });

            listV1.ItemsSource = departments;
            listV1.SelectedItem = departments[0];
            Department department = listV1.SelectedItem as Department;
            listV2.ItemsSource = department.subsidiaryDepartments;
            listV2.SelectedItem = department.subsidiaryDepartments[0];
            SubsidiaryDepartment subsidiaryDepartment = listV2.SelectedItem as SubsidiaryDepartment;
            listV3.ItemsSource = subsidiaryDepartment.personnels;
            listV3.SelectedItem = subsidiaryDepartment.personnels[0];



            ToolbarItems.Add(new ToolbarItem("完成", null, HandleAction));

        }
        private class Department{

            public string name
            {
                get;
                set;
            }

            public ObservableCollection<SubsidiaryDepartment> subsidiaryDepartments {
                get;
                set; 
            }
        }

        private class SubsidiaryDepartment{
            public string name
            {
                get;
                set;
            }

            public ObservableCollection<Personnel> personnels
            {
                get;
                set;
            }
        }

        private class Personnel
        {
            public string name
            {
                get;
                set;
            }           
        }
    }
}
