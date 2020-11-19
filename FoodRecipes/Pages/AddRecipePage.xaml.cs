using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FoodRecipes.Utilities;
using System.Windows.Forms;
using System.Diagnostics;
using FoodRecipes.Converter;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using System.Data.SqlClient;

namespace FoodRecipes.Pages
{
	/// <summary>
	/// Interaction logic for AddRecipePage.xaml
	/// </summary>
	public partial class AddRecipePage : Page
	{
		public delegate void BackToHomeHandler();
		public event BackToHomeHandler BackToHome;

		private DBUtilities _dbUtilitiesInstance = DBUtilities.GetDBInstance();
		private AppUtilities _appUtilities = new AppUtilities();

		private AbsolutePathConverter _absolutePathConverter = new AbsolutePathConverter();

		private Recipe recipe = new Recipe();
		private int totalStep = 0;

		public class MyImage: INotifyPropertyChanged
        {
			public string ImageSource { get; set; }
			public int ImageIndex { get; set; }

			public event PropertyChangedEventHandler PropertyChanged;
		}

		BindingList<MyImage> myImages = new BindingList<MyImage>();

		public AddRecipePage()
		{
			InitializeComponent();

			////Show snack bar
			//notiMessageSnackbar.MessageQueue.Enqueue("Test", "BACK HOME", () => { BackHome(); });
		}

		private void BackHome()
		{
			BackToHome?.Invoke();
		}

		private void avatarImagePickerButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void levelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void groupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void hourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void hourTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			//Regex meaning: not match any number digit zero or many times
			var pattern = "[^0-9]+";
			var regex = new Regex(pattern);

			//if true -> input event has handled (skiped this character)
			e.Handled = regex.IsMatch(e.Text);
		}

		private void relativeImagePickerButton_Click(object sender, RoutedEventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
				openFileDialog.Multiselect = true;
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
				openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.ico)|*.png;*.jpeg;*.jpg;*.ico";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					var imageIdx = 0;

					if (myImages.Count > 0)
					{
						myImages.Clear();
					}
					else
					{
						//Do nothing
					}

					foreach (var fileName in openFileDialog.FileNames)
					{
						MyImage myImage = new MyImage();

						myImage.ImageSource = fileName;
						myImage.ImageIndex = imageIdx++;
						Debug.WriteLine(imageIdx);

						myImages.Add(myImage);
					}

					relativeImageStepListView.Visibility = Visibility.Visible;
					relativeImageStepListView.ItemsSource = myImages;
				}
			}
		}

		private void deleteRelativeImageInListButton_Click(object sender, RoutedEventArgs e)
		{
			var clickedButton = (System.Windows.Controls.Button)sender;

			Debug.WriteLine(clickedButton.Tag);

			myImages.RemoveAt(int.Parse(clickedButton.Tag.ToString()));

			updateRelativeImageIndex();
		}

		private void updateRelativeImageIndex()
		{
			var index = 0;

			foreach (var image in myImages)
			{
				image.ImageIndex = index++;
			}

			if (myImages.Count == 0)
			{
				relativeImageStepListView.Visibility = Visibility.Collapsed;
			}
			else
			{
				relativeImageStepListView.ItemsSource = myImages;
			}
		}

		private void addStepButton_Click(object sender, RoutedEventArgs e)
		{
			if (detailStepTextBox.Text.Length == 0)
			{
				notiMessageSnackbar.MessageQueue.Enqueue("Không được bỏ trống chi tiết thực hiện", "Cancel", () => { });
			}
			else {
				messageNotFoundContainer.Visibility = Visibility.Collapsed;

				++totalStep;

				Step step = new Step();

				step.NO_STEP = totalStep;

				if (step.NO_STEP < 10)
                {
					step.NO_STEP_FOR_BINDING = $"0{step.NO_STEP}";
                } 
				else
                {
					step.NO_STEP_FOR_BINDING = $"{step.NO_STEP}";
				}

				step.DETAIL = detailStepTextBox.Text;

				foreach (var myImage in myImages)
				{
					StepImage stepImage = new StepImage();

					stepImage.NO_STEP = step.NO_STEP;
					stepImage.LINK_IMAGES = myImage.ImageSource;

					step.StepImages.Add(stepImage);
				}

				step.STEP_IMAGES_LIST_FOR_BINDING = step.StepImages.ToList();

				recipe.Steps.Add(step);

				stepsPreviewListView.ItemsSource = recipe.Steps.ToList();

				relativeImageStepListView.ItemsSource = null;

				detailStepTextBox.Text = "";

				relativeImageStepListView.Visibility = Visibility.Collapsed;
			}
		}

		private void addShoppingButton_Click(object sender, RoutedEventArgs e)
		{
			Igredient igredient = new Igredient();

			igredient.NAME = igredientNameTextBox.Text;
			if (igredient.NAME.Length == 0)
            {
				notiMessageSnackbar.MessageQueue.Enqueue("Không được bỏ trống tên nguyên liệu", "Cancel", () => { });
				return;
			} 
			else
            {
				//Do Nothing
            }

			igredient.QUANTITY = igredientQuantityTextBox.Text;
			if (igredient.QUANTITY.Length == 0)
			{
				notiMessageSnackbar.MessageQueue.Enqueue("Không được bỏ trống số lượng nguyên liệu", "Cancel", () => { });
				return;
			}
			else
			{
				//Do Nothing
			}

			igredientNameTextBox.Text = "";
			igredientQuantityTextBox.Text = "";

			recipe.Igredients.Add(igredient);

			igredientsListView.ItemsSource = recipe.Igredients.ToList();
		}

		private void cancelAddRecipeButton_Click(object sender, RoutedEventArgs e)
		{
			clearForm();
			notiMessageSnackbar.MessageQueue.Enqueue("Empty Form", "OK", () => { });
		}

		private void saveRecipeButton_Click(object sender, RoutedEventArgs e)
		{
			recipe.ID_RECIPE = _dbUtilitiesInstance.GetMaxIDRecipe() + 1;

			//Check name
			if (recipeNameTextBox.Text.Length == 0)
			{
				notiMessageSnackbar.MessageQueue.Enqueue("Không được bỏ trống tên món ăn", "Cancel", () => { });
				return;
			}
			else
			{
				recipe.NAME = recipeNameTextBox.Text;
			}

			//check description
			if (recipeDescriptionTextBox.Text.Length == 0)
			{
				notiMessageSnackbar.MessageQueue.Enqueue("Không được bỏ trống phần mô tả", "Cancel", () => { });
				return;
			}
			else
			{
				recipe.DESCRIPTION = recipeDescriptionTextBox.Text;
			}

			//Allow to empty
			recipe.LINK_VIDEO = linkVideoTextBox.Text;

			//Check empty Avatar
			if (avatarImage.Source.ToString() == FindResource("IconEmptyAvt").ToString())
			{
				notiMessageSnackbar.MessageQueue.Enqueue("Không được bỏ trống hình đại diện", "Cancel", () => { });
				return;
			}


			if (isValidTime())
			{
 				recipe.TIME = int.Parse(hourTextBox.Text) * 60 + int.Parse(minuteTextBox.Text);
			}
			else
			{
				if (int.Parse(minuteTextBox.Text) >= 60)
				{
					notiMessageSnackbar.MessageQueue.Enqueue("Số phút thực hiện phải nhỏ hơn 60", "Cancel", () => { });
				}
				else
                {
					notiMessageSnackbar.MessageQueue.Enqueue("Không được bỏ trống thời gian hoàn thành", "Cancel", () => { });
				}

				hourTextBox.Text = "";
				minuteTextBox.Text = "";

				return;
			}

			recipe.FOOD_GROUP = groupComboBox.Text;
			recipe.FOOD_LEVEL = levelComboBox.SelectedIndex;

			if (recipe.Igredients.Count == 0)
			{
				notiMessageSnackbar.MessageQueue.Enqueue("Không được bỏ trống nguyên liệu", "Cancel", () => { });
				return;
			}
			else 
			{
				//Do Nothing
			}

			if (recipe.Steps.Count == 0) 
			{
				notiMessageSnackbar.MessageQueue.Enqueue("Không được bỏ trống các bước thực hiện", "Cancel", () => { });
				return;
			} 
			else
            {
				try
				{
					var srcAvatar = avatarImage.Source.ToString();
					recipe.LINK_AVATAR = _appUtilities.getTypeOfImage(srcAvatar);

					var today = DateTime.Now;

					Debug.WriteLine(recipe.NAME);
			

					if (_dbUtilitiesInstance.InsertRecipe(recipe.ID_RECIPE, recipe.NAME, recipe.DESCRIPTION, recipe.LINK_VIDEO, recipe.LINK_AVATAR, recipe.TIME, recipe.FOOD_GROUP, recipe.FOOD_LEVEL, false, false, today) == 1)
                    {
						_appUtilities.createIDDirectory(recipe.ID_RECIPE);

						_appUtilities.copyImageToIDDirectory(recipe.ID_RECIPE, srcAvatar, "avatar");

						foreach (var igredient in recipe.Igredients)
						{
							try
							{
								_dbUtilitiesInstance.InsertIgredient(recipe.ID_RECIPE, igredient.NAME, igredient.QUANTITY);

							}
							catch (Exception excep)
							{
								Debug.WriteLine(excep.Message);
							}

							var steps = recipe.Steps.ToList();

							for (int no_step = 1; no_step <= steps.Count; ++no_step)
							{
								var step = steps[no_step - 1];

								step.ID_RECIPE = recipe.ID_RECIPE;

								try
								{
									_dbUtilitiesInstance.InsertStep(recipe.ID_RECIPE, no_step, step.DETAIL);
								}
								catch (Exception excep)
								{
									Debug.WriteLine(excep.Message);
								}

								var images = step.StepImages.ToList();

								for (int no_image = 1; no_image <= images.Count; ++no_image)
								{
									var image = images[no_image - 1];

									image.ID_RECIPE = recipe.ID_RECIPE;

									var srcImage = image.LINK_IMAGES;
									var linkImage = $"{no_step}_{no_image}";

									try
									{
										_appUtilities.copyImageToIDDirectory(recipe.ID_RECIPE, srcImage, linkImage);
									}
									catch (Exception excep)
									{
										Debug.WriteLine(excep.Message);
									}

									try
									{
										_dbUtilitiesInstance.InsertStepImages(recipe.ID_RECIPE, no_step, $"{linkImage}.{_appUtilities.getTypeOfImage(srcImage)}");
									}
									catch (Exception excep)
									{
										Debug.WriteLine(excep.Message);
									}
								}

								notiMessageSnackbar.MessageQueue.Enqueue("Thêm món ăn thành công", "BACK HOME", BackHome);
							}
						}
					}
					else
                    {
						notiMessageSnackbar.MessageQueue.Enqueue($"Đã tồn tại {recipe.NAME}", "BACK HOME", BackHome);
					}
				}
				catch (Exception excep) 
				{
					Debug.WriteLine(excep.Message);
				}
            }

			clearForm();

			totalStep = 0;
		}

		private void avatarPickerFrameButton_Click(object sender, RoutedEventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
				openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.ico)|*.png;*.jpeg;*.jpg;*.ico";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{ 
					BitmapImage bitmap = new BitmapImage();

					bitmap.BeginInit();
					bitmap.CacheOption = BitmapCacheOption.OnLoad;
					bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Relative);
					bitmap.EndInit();

					avatarImage.Source = bitmap;
				}
			}
		}

		private bool isValidTime() {
			var result = true;

			if (hourTextBox.Text == "")
			{
				hourTextBox.Text = "0";
			}
			else 
			{ 
				//Do nothing	
			}

			if (minuteTextBox.Text == "")
			{
				minuteTextBox.Text = "0";
			}
			else
			{
				//Do nothing	
			}

			if (hourTextBox.Text == "0" && minuteTextBox.Text == "0") {
				result = false;
			}

			if (int.Parse(minuteTextBox.Text) >= 60)
            {
				result = false;
			}

			return result;
		}

		private void clearForm()
        {
			recipe = new Recipe();

			messageNotFoundContainer.Visibility = Visibility.Visible;

			recipeNameTextBox.Text = "";
			recipeDescriptionTextBox.Text = "";
			linkVideoTextBox.Text = "";
			
			avatarImage.Source = new BitmapImage(new Uri(FindResource("IconEmptyAvt").ToString()));
			hourTextBox.Text = "";
			minuteTextBox.Text = "";
			groupComboBox.SelectedIndex = 0;
			levelComboBox.SelectedIndex = 0;
			igredientNameTextBox.Text = "";
			igredientQuantityTextBox.Text = "";
			igredientsListView.ItemsSource = null;
			detailStepTextBox.Text = "";
			relativeImageStepListView.ItemsSource = null;
			stepsPreviewListView.ItemsSource = null;
		}

        private void openLocalVideoButton_Click(object sender, RoutedEventArgs e)
        {
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
				openFileDialog.Filter = "Video files (*.mkv)|*.mkv";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					linkVideoTextBox.Text = openFileDialog.FileName;
				}
			}
		}
    }
}
