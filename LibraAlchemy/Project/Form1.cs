using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraAlchemy
{
    public partial class Form1 : Form
    {
        private Alchemist alchemist;
        private string EditProductLastAction;
        private Effect EditProductLastEffect;
        private List<Object> InPotionList;
        private int UpCounter;

        public Form1()
        {
            InitializeComponent();
            alchemist = new Alchemist();
            EditProductLastAction = "";
            EditProductLastEffect = null;
            UpdateAlchemyMasterTab();
            UpCounter = 0;
            SetToolTips();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as TabControl).SelectedIndex)
            {
                case 0:
                    UpdateAlchemyMasterTab();
                    break;
                case 1:
                    UpdateEditProductTab();
                    break;
                case 2:
                    UpdateEditIngredientTab();
                    break;
                case 3:
                    UpdateEditPotionTab();
                    break;
                case 4:
                    UpdateEffectInfoTab();
                    break;
                case 5:
                    UpdateEditSpecialTab();
                    break;
                case 6:
                    UpdatePowerLevelTab();
                    break;
                case 7:
                    UpdateAddPotionTab();
                    break;
                default:
                    break;
            }
        }

        //
        //Master Tab
        //
        private void UpdateAlchemyMasterTab()
        {
            try
            {
                //In potion list is the list of all the products and specials in the potion.
                InPotionList = new List<object>();
                //Update the alchemist
                alchemist.GetAllIngredients();
                alchemist.GetAllProcedures();
                alchemist.GetAllSpecials();
                alchemist.GetAllPotions();
                //Clear the Controls from all items and text
                MasterAlchemyLV.Items.Clear();
                MasterEffectLV.Items.Clear();
                MasterSimilarPotionLV.Items.Clear();
                MasterIngredientCB.Items.Clear();
                MasterTechniqueCB.Items.Clear();
                MasterSpecialCB.Items.Clear();
                //Disable the form of the potion details
                DisablePotionForm();
                //Fill the Comboboxes with data
                MasterIngredientCB.Items.AddRange(alchemist.Ingredient_List.ToArray());
                MasterIngredientCB.DisplayMember = "Name";
                MasterTechniqueCB.Items.AddRange(alchemist.Procedure_List.ToArray());
                MasterTechniqueCB.DisplayMember = "Name";
                MasterSpecialCB.Items.AddRange(alchemist.Special_List.ToArray());
                MasterSpecialCB.DisplayMember = "Name";
            }
            catch (Exception e){
                MessageBox.Show(e.ToString());
            }
 
        }

        private void DisablePotionForm()
        {
            MasterPotionNameTB.Text = "";
            MasterCreatorNameTB.Text = "";
            MasterPotionDescriptionTB.Text = "";
            MasterPotionNameTB.ReadOnly = true;
            MasterCreatorNameTB.ReadOnly = true;
            MasterPotionDescriptionTB.ReadOnly = true;
            //Reset the add potion button
            MasterAddPotionButton.Enabled = false;
            MasterAddPotionButton.ForeColor = System.Drawing.Color.Gray;
        }

        private void MasterAddProductButton_Click(object sender, EventArgs e)
        {
            if (MasterIngredientCB.SelectedIndex < 0 || MasterTechniqueCB.SelectedIndex < 0)
                return;
            Product prod = alchemist.GetProduct(MasterIngredientCB.SelectedItem as Ingredient, MasterTechniqueCB.SelectedItem as Procedure);
            //Add the Product into the list of potion items
            InPotionList.Add(prod);
            RefreshPotionItems();
        }

        private void RemoveProductFromMasterLVClick(object sender, EventArgs e)
        {
            //The button contains the product it is connected to
            Product prod = (sender as Button).Tag as Product;
            InPotionList.Remove(prod);
            RefreshPotionItems();
        }

        private void MasterAddSpecialButton_Click(object sender, EventArgs e)
        {
            if (MasterSpecialCB.SelectedIndex < 0)
            {
                return;
            }
            Special spec = MasterSpecialCB.SelectedItem as Special;
            //Add the special into the list of potion items
            InPotionList.Add(spec);
            //Remove the special from the CB, because it can't appear twice in the potion
            MasterSpecialCB.Items.Remove(spec);
            RefreshPotionItems();
        }


        private void RemoveSpecialFromMasterLVClick(object sender, EventArgs e)
        {
            Special spec = (sender as Button).Tag as Special;
            InPotionList.Remove(spec);
            MasterSpecialCB.Items.Add(spec);
            RefreshPotionItems();
        }

        private void RefreshPotionItems()
        {
            //Refresh the list's products (Specials do not need refresh) 
            foreach (Object item in InPotionList)
            {
                if (item is Product)
                {
                    alchemist.RefreshProduct(item as Product);
                }
            }
            //Refresh the controls.
            MasterAddPotionButton.ForeColor = System.Drawing.Color.Black;
            //After the refresh, we can ask alchemist to make the new potion.
            MakePotion();
            //After alchemist is done, we can apply the changes on the tab
            DisplayPotion();
        }

        private void MakePotion()
        {
            alchemist.MakePotion(InPotionList);
        }

        private void DisplayPotion()
        {
            Button b;
            ListViewItem LVI;
            Product prod;
            Special spec;
            int index = 0;
            //Clear all the items in the listview and insert them again.
            MasterAlchemyLV.Items.Clear();
            foreach (Object obj in InPotionList)
            {
                if (obj is Special)
                {
                    spec = obj as Special;
                    LVI = new ListViewItem(spec.Name);
                    LVI.ToolTipText = spec.ToString();
                    LVI.Font = new System.Drawing.Font("David", 11.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    MasterAlchemyLV.Items.Add(LVI);
                    b = new Button();
                    b.BackgroundImage = Properties.Resources.red_minus;
                    b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                    b.BackColor = SystemColors.Control;
                    b.Click += new EventHandler(RemoveSpecialFromMasterLVClick);
                    b.Tag = spec;
                    MasterAlchemyLV.AddEmbeddedControl(b, 2, LVI.Index);
                }
                else if (obj is Product)
                {
                    prod = obj as Product;
                    LVI = new ListViewItem(prod.GetProdName());
                    LVI.ToolTipText = prod.ToString();
                    LVI.Font = new System.Drawing.Font("David", 11.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    LVI.UseItemStyleForSubItems = false;
                    MasterAlchemyLV.Items.Add(LVI);
                    b = new Button();
                    b.BackgroundImage = Properties.Resources.red_minus;
                    b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                    b.BackColor = SystemColors.Control;
                    b.Click += new EventHandler(RemoveProductFromMasterLVClick);
                    b.Tag = prod;
                    MasterAlchemyLV.AddEmbeddedControl(b, 2, LVI.Index);
                    //Now add the effects in the right color to the LV
                    index = 0;
                    while (index < prod.Effects.Count)
                    {
                        switch (index % 3)
                        {
                            case 0:
                                {
                                    LVI = new ListViewItem(prod.Effects[index].Name);
                                    LVI.ForeColor = prod.Effects[index].FontColor;
                                    LVI.UseItemStyleForSubItems = false;
                                    MasterAlchemyLV.Items.Add(LVI);
                                    index ++;
                                    break;
                                }
                            case 1: case 2:
                                {
                                    LVI.SubItems.Add(new ListViewItem.ListViewSubItem(LVI, prod.Effects[index].Name, prod.Effects[index].FontColor, LVI.BackColor, LVI.Font));
                                    index++;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
            //Clear and refill the final effects in the potion
            MasterEffectLV.Items.Clear();
            index = 0;
            foreach (FinalEffect e in alchemist.Current_Potion.Effects)
            {
                LVI = new ListViewItem(e.Name);
                if (index <= 4)
                    LVI.Font = new System.Drawing.Font("David", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                if (e.Commulative)
                    LVI.Text += "(" + e.Level.ToString() + ")";
                LVI.ForeColor = e.FontColor;
                LVI.ToolTipText = e.Description;
                LVI.ToolTipText += Environment.NewLine + "הופיע בשילובים: | ";
                foreach (Effect parent in e.Parents)
                    LVI.ToolTipText += (parent.Owner as Product).GetProdName() + " | ";
                if (e.ChangedBySpecial)
                    LVI.ToolTipText += Environment.NewLine + "השתנה מ" + e.OldName + " ל" + e.Name + " בעקבות שימוש ב" + e.Changer.Name;
                if (index > 4)
                    LVI.ToolTipText += Environment.NewLine + "ההשפעה חלשה מדי ביחס להשפעות האחרות בכדי להיכנס לשיקוי.";
                MasterEffectLV.Items.Add(LVI);
                index++;
            }
            //Check Similar Potions
            MasterSimilarPotionLV.Items.Clear();
            List<Potion> sim_pots = alchemist.GetSimilarPotions(alchemist.Current_Potion.Effects);
            foreach (Potion pot in sim_pots)
            {
                LVI = new ListViewItem(pot.Name);
                LVI.Font = new System.Drawing.Font("David", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                LVI.ToolTipText = pot.ToString();
                MasterSimilarPotionLV.Items.Add(LVI);
            }
            //Check if potion exists
            Potion p = alchemist.GetPotionByEffects(alchemist.Current_Potion.Effects);
            if (alchemist.Current_Potion.Effects.Count > 0)
            {
                if (p == null)
                {
                    //No potion exists, meaning we can create a new potion.
                    MasterAddPotionButton.Enabled = true;
                    if (alchemist.Current_Potion.Effects.Count < 3)
                        MasterAddPotionButton.ForeColor = System.Drawing.Color.Red;
                    else
                        MasterAddPotionButton.ForeColor = System.Drawing.Color.Black;
                    MasterPotionNameTB.Text = "";
                    MasterCreatorNameTB.Text = "";
                    MasterPotionDescriptionTB.Text = "";
                    MasterPotionNameTB.ReadOnly = false;
                    MasterCreatorNameTB.ReadOnly = false;
                    MasterPotionDescriptionTB.ReadOnly = false;
                }
                else
                {
                    //The potion exists, which means we can't add the potion.
                    DisablePotionForm();
                    MasterPotionNameTB.Text = p.Name;
                    MasterCreatorNameTB.Text = p.Creator;
                    MasterPotionDescriptionTB.Text = p.Description;
                }
            }
            else
            {
                //Potion has no effects in it
                DisablePotionForm();
            }
        }

        private void MasterAddPotionButton_Click(object sender, EventArgs e)
        {
            if (MasterPotionNameTB.Text != "" && alchemist.Current_Potion.Effects.Count > 0)
            {
                alchemist.Current_Potion.Name = MasterPotionNameTB.Text;
                alchemist.Current_Potion.Description = MasterPotionDescriptionTB.Text;
                alchemist.Current_Potion.Creator = MasterCreatorNameTB.Text;
                alchemist.AddNewPotion(alchemist.Current_Potion);
                MasterAddPotionButton.Enabled = false;
                MasterAddPotionButton.ForeColor = System.Drawing.Color.Gray;
                MasterPotionNameTB.ReadOnly = true;
                MasterCreatorNameTB.ReadOnly = true;
                MasterPotionDescriptionTB.ReadOnly = true;
            }
        }

        private void SetToolTips()
        {
            ToolTip yourToolTip = new ToolTip();
            yourToolTip.ToolTipIcon = ToolTipIcon.Info;
            yourToolTip.IsBalloon = true;
            yourToolTip.ShowAlways = true;
            label52.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            yourToolTip.SetToolTip(label52, "שיקוי פולימיצי: \n5 כנפי שפריריות \n4 עלוקות\nיבלית שנקטפה בליל ירח מלא\n10 גרם אבקת אונטנרה\n6 רצועות של עור בומסלאנג\nחלק מהגוף שאתם רוצים להשתנות אליו");
            
        }

        //
        // Edit Product and Effect Tab
        //

        private void UpdateEditProductTab()
        {
            //Set them to default
            EditProductLastAction = "";
            EditProductLastEffect = null;
            //Update Alchemist
            alchemist.GetAllIngredients();
            alchemist.GetAllProcedures();
            alchemist.GetAllEffects();
            //Clean the Product Edit boxes from data
            EditProductIngredientCB.Items.Clear();
            EditProductProcedureCB.Items.Clear();
            EditProductEffectCB.Items.Clear();
            EditProductEffectsLV.Items.Clear();
            EditProductMainEffectCB.Items.Clear();
            EditProductCommentTB.Text = "";
            // and refill them...
            EditProductIngredientCB.Items.AddRange(alchemist.Ingredient_List.ToArray());
            EditProductIngredientCB.DisplayMember = "Name";
            EditProductProcedureCB.Items.AddRange(alchemist.Procedure_List.ToArray());
            EditProductProcedureCB.DisplayMember = "Name";
            //Before the selection of the two, the EffectsCB and MainEffectCB are not available, so we do not fill them yet.
            //Clean the fields Effect Edit data and boxes
            EditEffectsNameTB.Text = "";
            EditEffectsDescriptionTB.Text = "";
            EditEffectsCommentTB.Text = "";
            EditEffectCommulativeCheckbox.Checked = false;
            //Empty and refill the CB
            EditEffectsEffectCB.Items.Clear();
            EditEffectsEffectCB.Items.AddRange(alchemist.Effect_List.ToArray());
            Effect e = new Effect();
            e.Name = "<השפעה חדשה>";
            EditEffectsEffectCB.Items.Add(e);
            EditEffectsEffectCB.DisplayMember = "Name";
            EditCountersEffectCB.Items.Clear();
            EditCountersEffectCB.Items.AddRange(alchemist.Effect_List.ToArray());
            EditCountersEffectCB.DisplayMember = "Name";
            
        }

        private void EditProductIngredientCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditProductIngredientCB.SelectedIndex >= 0 && EditProductProcedureCB.SelectedIndex >= 0)
            {
                EditProduct_ProductSelected();
            }
        }

        private void EditProductProcedureCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditProductIngredientCB.SelectedIndex >= 0 && EditProductProcedureCB.SelectedIndex >= 0)
            {
                EditProduct_ProductSelected();
            }
        }

        private void EditProduct_ProductSelected()
        {
            Button b;
            ListViewItem LVI;
            int index = -1;
            //Cleanup before populating it again.
            EditProductEffectCB.Items.Clear();
            EditProductEffectsLV.Items.Clear();
            EditProductMainEffectCB.Items.Clear();
            EditProductCommentTB.Text = "";
            //Ask Alchemist to hand us a product, if it exists we will get it, otherwise we'll get a product with -1 for ID.
            Product prod = alchemist.GetProduct(EditProductIngredientCB.SelectedItem as Ingredient, EditProductProcedureCB.SelectedItem as Procedure);
            if (prod.ID >= 0)
            {
                //Temp list of all the effects
                List<Effect> temp_list = new List<Effect>();
                foreach (Effect e in alchemist.Effect_List)
                    temp_list.Add(e);
                //
                foreach (Effect e in prod.Effects)
                {
                    //Remove all the effects that are already in the Product or that counter effects in the product from temp_list.
                    temp_list.RemoveAll(item => item.ID == e.ID);
                    if (e.Counters_IDs.Any())
                        foreach (int ID in e.Counters_IDs)
                        {
                            temp_list.RemoveAll(item => item.ID == ID);
                        }
                    //Add the effects of the product to the ListView
                    LVI = new ListViewItem(e.Name);
                    //Check if main effect to put new font
                    if (e.ID == prod.Main_Effect_ID)
                        LVI.Font = new System.Drawing.Font("David", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    LVI.ToolTipText = e.Description;
                    EditProductEffectsLV.Items.Add(LVI);
                    b = new Button();
                    b.BackgroundImage = Properties.Resources.red_minus;
                    b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                    b.BackColor = SystemColors.Control;
                    //Add event handler...
                    b.Click += new EventHandler(RemoveEffectFromProduct_Click);
                    b.Tag = e;
                    EditProductEffectsLV.AddEmbeddedControl(b, 1, LVI.Index);
                    //Add the effects of the product to the MainEffectCB and determine the index
                    if (e.ID == prod.Main_Effect_ID)
                    {
                        index = EditProductMainEffectCB.Items.Count;
                    }
                    EditProductMainEffectCB.Items.Add(e);
                }
                //Change the index of EditProductMainEffectCB to be that of the main index, if no main index chosen the first one would be chosen.
                EditProductMainEffectCB.SelectedIndex = index;
                EditProductMainEffectCB.DisplayMember = "Name";
                //Add comment to textbox
                EditProductCommentTB.Text = prod.Comment;

                //The effects that you can still add into the product are taken from temp_list
                EditProductEffectCB.Items.AddRange(temp_list.ToArray());
                EditProductEffectCB.DisplayMember = "Name";
            }
            else
            {
                //If the product does not exist we will simply prepare the list of all the effects. Once they press the add button on an empty product
                //we will put the product into the DB And call this function again.
                EditProductEffectCB.Items.AddRange(alchemist.Effect_List.ToArray());
                EditProductEffectCB.DisplayMember = "Name";
            }
        }

        private void RemoveEffectFromProduct_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            Effect effect = b.Tag as Effect;
            alchemist.RemoveEffectFromProduct(effect, effect.Owner as Product);
            EditProductLastAction = "Delete";
            EditProductLastEffect = effect;
            EditProduct_ProductSelected();
        }

        private void EditProductAddEffectButton_Click(object sender, EventArgs e)
        {
            //If no combination is selected, do nothing.
            if (EditProductIngredientCB.SelectedIndex < 0 || EditProductProcedureCB.SelectedIndex < 0 || EditProductEffectCB.SelectedIndex < 0)
                return;
            //Ask Alchemist to hand us a product, if it exists we will get it, otherwise we'll get a product with -1 for ID.
            Ingredient ing = EditProductIngredientCB.SelectedItem as Ingredient;
            Procedure proc = EditProductProcedureCB.SelectedItem as Procedure;
            Product prod = alchemist.GetProduct(ing,proc);
            Effect effect = EditProductEffectCB.SelectedItem as Effect;
            if (prod.ID >= 0)
            {
                alchemist.AddEffectToProduct(effect, prod);
            }
            else
            {
                //The first effect becomes the main effect
                prod.Main_Effect_ID = effect.ID;
                //Add Product to DB
                alchemist.AddNewProduct(prod);
                //Select Product from DB (So it'll have ID)
                prod = alchemist.GetProduct(ing,proc);
                //Add effect to the Product in DB
                alchemist.AddEffectToProduct(effect, prod);

            }
            EditProductLastAction = "Add";
            effect.Owner = prod;
            EditProductLastEffect = effect;
            EditProduct_ProductSelected();
        }

        private void EditProductUndoButton_Click(object sender, EventArgs e)
        {
            if (EditProductLastAction == "" || EditProductLastEffect == null)
            {
                return;
            }
            Product p = EditProductLastEffect.Owner as Product;
            if (EditProductLastAction == "Add")
                alchemist.RemoveEffectFromProduct(EditProductLastEffect, p);
            else
                if (EditProductLastAction == "Delete")
                    alchemist.AddEffectToProduct(EditProductLastEffect, p);
            EditProductLastAction = "";
            EditProductLastEffect = null;
            EditProduct_ProductSelected();
        }

        private void EditProductSaveButton_Click(object sender, EventArgs e)
        {
            //If no product is selcted, do nothing.
            if (EditProductIngredientCB.SelectedIndex < 0 || EditProductProcedureCB.SelectedIndex < 0)
                return;
            //Ask Alchemist to hand us a product, if it exists we will get it, otherwise we'll get a product with -1 for ID.
            Ingredient ing = EditProductIngredientCB.SelectedItem as Ingredient;
            Procedure proc = EditProductProcedureCB.SelectedItem as Procedure;
            Product prod = alchemist.GetProduct(ing, proc);
            if (EditProductMainEffectCB.SelectedIndex >= 0)
                prod.Main_Effect_ID = (EditProductMainEffectCB.SelectedItem as Effect).ID;
            prod.Comment = EditProductCommentTB.Text;
            if (prod.ID >= 0)
            {
                alchemist.UpdateProduct(prod);
            }
            else
            {
                alchemist.AddNewProduct(prod);
            }
            EditProduct_ProductSelected();
        }

        private void RefreshEditEffectsEffectCB()
        {
            string name = "";
            alchemist.GetAllEffects();
            if (EditEffectsEffectCB.SelectedIndex != -1)
            {
                name = (EditEffectsEffectCB.SelectedItem as Effect).Name;
            }
            EditEffectsEffectCB.Items.Clear();
            EditEffectsEffectCB.Items.AddRange(alchemist.Effect_List.ToArray());
            Effect e = new Effect();
            e.Name = "<השפעה חדשה>";
            EditEffectsEffectCB.Items.Add(e);
            EditEffectsEffectCB.DisplayMember = "Name";
            if (name != "")
            {
                EditEffectsEffectCB.SelectedIndex = alchemist.Effect_List.FindIndex(item => item.Name == name);
            }
            name = "";
            if (EditCountersEffectCB.SelectedIndex != -1)
            {
                name = (EditCountersEffectCB.SelectedItem as Effect).Name;
            }
            EditCountersEffectCB.Items.Clear();
            EditCountersEffectCB.Items.AddRange(alchemist.Effect_List.ToArray());
            EditCountersEffectCB.DisplayMember = "Name";
            if (name != "")
            {
                EditCountersEffectCB.SelectedIndex = alchemist.Effect_List.FindIndex(item => item.Name == name);
            }
        }

        private void EditEffectsEffectCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditEffectsEffectCB.SelectedIndex < 0)
                return;
            Effect effect = EditEffectsEffectCB.SelectedItem as Effect;
            if (effect.ID >= 0)
                EditEffectsNameTB.Text = effect.Name;
            else
            {
                EditEffectsNameTB.Text = "";
            }
            EditEffectsDescriptionTB.Text = effect.Description;
            EditEffectsCommentTB.Text = effect.Comment;
            EditEffectCommulativeCheckbox.Checked = effect.Commulative;
        }

        private void EditEffectsSaveButton_Click(object sender, EventArgs e)
        {
            if (EditEffectsEffectCB.SelectedIndex < 0)
                return;
            Effect effect = EditEffectsEffectCB.SelectedItem as Effect;
            effect.Name = EditEffectsNameTB.Text;
            effect.Description = EditEffectsDescriptionTB.Text;
            effect.Comment = EditEffectsCommentTB.Text;
            effect.Commulative = EditEffectCommulativeCheckbox.Checked;
            if (effect.ID >= 0)
            {
                alchemist.UpdateEffect(effect);
            }
            else
            {
                //Add new Effect
                alchemist.AddNewEffect(effect);
            }
            //Make sure the effect list of the alchemist will still be up to date.
            RefreshEditEffectsEffectCB();
            //Need to update the selceted Product as well because they are in the same page and the effects should be as updated as possible.
            if (EditProductIngredientCB.SelectedIndex >= 0 && EditProductProcedureCB.SelectedIndex >= 0)
            {
                EditProduct_ProductSelected();
            }
        }

        private void EditCountersEffectCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditCountersEffectCB.SelectedIndex < 0)
                return;
            EditCounterCounterSelected();
        }

        private void EditCounterCounterSelected()
        {
            Button b;
            ListViewItem LVI;
            //First thing's First - Empty our Listview and CounterComboBox
            EditCountersCounterCB.Items.Clear();
            EditCountersLV.Items.Clear();
            Effect selected_effect = EditCountersEffectCB.SelectedItem as Effect;
            foreach (Effect effect in alchemist.Effect_List)
            {
                if (effect.ID == selected_effect.ID)
                {
                    //Do nothing
                }
                else
                {
                    if (selected_effect.Counters_IDs.IndexOf(effect.ID) != -1)
                    {
                        //This means this effect is indeed in the counter list of the selected effect!
                        //Lets add it to the listview!
                        LVI = new ListViewItem(effect.Name);
                        LVI.ToolTipText = effect.Description;
                        EditCountersLV.Items.Add(LVI);
                        b = new Button();
                        b.BackgroundImage = Properties.Resources.red_minus;
                        b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                        b.BackColor = SystemColors.Control;
                        //Add event handler...
                        b.Click += new EventHandler(RemoveEffectFromCounters_Click);
                        b.Tag = effect;
                        EditCountersLV.AddEmbeddedControl(b, 1, LVI.Index);
                    }
                    else
                    {
                        //Not in the counter list of our selcted effect
                        //Lets add it to the combobox of the counters!
                        EditCountersCounterCB.Items.Add(effect);
                    }
                }
            }
            EditCountersCounterCB.DisplayMember = "Name";
        }

        private void RemoveEffectFromCounters_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            Effect removed_effect = b.Tag as Effect;
            Effect origin_effect = EditCountersEffectCB.SelectedItem as Effect;
            alchemist.RemoveCounterFromEffect(origin_effect, removed_effect);
            EditCounterCounterSelected();
            //Make sure the effect list of the alchemist will still be up to date.
            RefreshEditEffectsEffectCB();
            //Need to update the selceted Product as well because they are in the same page and the effects should be as updated as possible.
            if (EditProductIngredientCB.SelectedIndex >= 0 && EditProductProcedureCB.SelectedIndex >= 0)
            {
                EditProduct_ProductSelected();
            }
        }

        private void EditCountersAddCounterButton_Click(object sender, EventArgs e)
        {
            if (EditCountersCounterCB.SelectedIndex < 0 || EditCountersEffectCB.SelectedIndex < 0)
                return;
            Effect added_effect = EditCountersCounterCB.SelectedItem as Effect;
            Effect origin_effect = EditCountersEffectCB.SelectedItem as Effect;
            alchemist.AddCounterToEffect(origin_effect, added_effect);
            EditCounterCounterSelected();
            //Make sure the effect list of the alchemist will still be up to date.
            RefreshEditEffectsEffectCB();
            //Need to update the selceted Product as well because they are in the same page and the effects should be as updated as possible.
            if (EditProductIngredientCB.SelectedIndex >= 0 && EditProductProcedureCB.SelectedIndex >= 0)
            {
                EditProduct_ProductSelected();
            }
        }

        //
        //Edit Ingredient and Procedure tab
        //

        private void UpdateEditIngredientTab()
        {
            //Ingredient Cleanup
            //Remove all items from combobox and puts new items from DB.
            EditIngredientCB.Items.Clear();
            alchemist.GetAllIngredients();
            EditIngredientCB.Items.AddRange(alchemist.Ingredient_List.ToArray());
            //Add a new items in case of new Ingredient
            Ingredient new_ing = new Ingredient();
            new_ing.Name = "<מרכיב חדש>";
            new_ing.ID = -1;
            EditIngredientCB.Items.Add(new_ing);
            EditIngredientCB.DisplayMember = "Name";
            EditIngredientCB.ValueMember = "ID";
            EditIngredientNameTB.Text = "";
            EditIngredientCommentTB.Text = "";
            //Procedure Cleanup
            EditTechCB.Items.Clear();
            alchemist.GetAllProcedures();
            EditTechCB.Items.AddRange(alchemist.Procedure_List.ToArray());
            Procedure new_pro = new Procedure();
            new_pro.Name = "<טכניקה חדשה>";
            new_pro.ID = -1;
            EditTechCB.Items.Add(new_pro);
            EditTechCB.DisplayMember = "Name";
            EditTechCB.ValueMember = "ID";
            EditTechNameTB.Text = "";
            EditTechCommentTB.Text = "";   
        }

        private void EditIngredientCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditIngredientCB.SelectedIndex < 0)
                return;
            Ingredient selected_ing = (EditIngredientCB.SelectedItem as Ingredient);
            if (selected_ing.ID >= 0)
            {
                EditIngredientNameTB.Text = selected_ing.Name;
                EditIngredientCommentTB.Text = selected_ing.Comment;
            }
            else
            {
                EditIngredientNameTB.Text = "";
                EditIngredientCommentTB.Text = "";
            }
        }

        private void EditIngredientButton_Click(object sender, EventArgs e)
        {
            if (EditIngredientCB.SelectedIndex < 0)
                return;
            Ingredient selected_ing = (EditIngredientCB.SelectedItem as Ingredient);
            int index = 0;
            selected_ing.Name = EditIngredientNameTB.Text;
            selected_ing.Comment = EditIngredientCommentTB.Text;
            if (!alchemist.UpdateIngredient(selected_ing))
                MessageBox.Show("There was some error with adding the ingredient to the Database.");
            else
            {
                UpdateEditIngredientTab();
                index = alchemist.Ingredient_List.FindIndex(item => item.Name == selected_ing.Name);
                EditIngredientCB.SelectedIndex = index;
            }
        }

        private void EditTechCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditTechCB.SelectedIndex < 0)
                return;
            Procedure selected_pro = (EditTechCB.SelectedItem as Procedure);
            if (selected_pro.ID >= 0)
            {
                EditTechNameTB.Text = selected_pro.Name;
                EditTechCommentTB.Text = selected_pro.Comment;
            }
            else
            {
                EditTechNameTB.Text = "";
                EditTechCommentTB.Text = "";
            }
        }

        private void EditTechniqueButton_Click(object sender, EventArgs e)
        {
            if (EditTechCB.SelectedIndex < 0)
                return;
            Procedure selected_pro = (EditTechCB.SelectedItem as Procedure);
            int index = 0;
            selected_pro.Name = EditTechNameTB.Text;
            selected_pro.Comment = EditTechCommentTB.Text;
            if (!alchemist.UpdateProcedure(selected_pro))
                MessageBox.Show("There was some error with adding the procedure to the Database.");
            else
            {
                UpdateEditIngredientTab();
                index = alchemist.Procedure_List.FindIndex(item => item.Name == selected_pro.Name);
                EditTechCB.SelectedIndex = index;
            }
        }

        //
        //Edit Potion Tab
        //

        private void UpdateEditPotionTab()
        {
            //Cleanup!
            EditPotionCB.Items.Clear();
            EditPotionNameTB.Text = "";
            EditPotionDescriptionTB.Text = "";
            EditPotionCreatorTB.Text = "";
            EditPotionEffectsLV.Items.Clear();
            EditPotionSaveButton.Enabled = false;
            EditPotionDeleteButton.Enabled = false;
            //Get all the potions to Alchemist
            alchemist.GetAllPotions();
            EditPotionCB.Items.AddRange(alchemist.Potion_List.ToArray());
            EditPotionCB.DisplayMember = "Name";
        }

        private void EditPotionCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditPotionCB.SelectedIndex < 0)
            {
                EditPotionSaveButton.Enabled = false;
                EditPotionDeleteButton.Enabled = false;
                return;
            }
            Potion p = EditPotionCB.SelectedItem as Potion;
            string name;
            ListViewItem LVI;
            //Set the data
            EditPotionNameTB.Text = p.Name;
            EditPotionDescriptionTB.Text = p.Description;
            EditPotionCreatorTB.Text = p.Creator;
            //Populate the listview
            EditPotionEffectsLV.Items.Clear();
            foreach (FinalEffect fe in p.Effects)
            {
                name = fe.Name;
                if (fe.Commulative)
                    name += "(" + fe.Level + ")";
                LVI = new ListViewItem(name);
                LVI.ToolTipText = fe.Description;
                EditPotionEffectsLV.Items.Add(LVI);
            }
            EditPotionSaveButton.Enabled = true;
            EditPotionDeleteButton.Enabled = true;
        }

        private void EditPotionSaveButton_Click(object sender, EventArgs e)
        {
            if (EditPotionCB.SelectedIndex < 0)
                return;
            Potion p = EditPotionCB.SelectedItem as Potion;
            p.Name = EditPotionNameTB.Text;
            p.Description = EditPotionDescriptionTB.Text;
            p.Creator = EditPotionCreatorTB.Text;
            int index = EditPotionCB.SelectedIndex;
            alchemist.UpdatePotion(p);
            UpdateEditPotionTab();
            EditPotionCB.SelectedIndex = index;
        }

        private void EditPotionDeleteButton_Click(object sender, EventArgs e)
        {
            if (EditPotionCB.SelectedIndex < 0)
                return;
            //Ask the user if he is certain he wishes to delete
            Potion p = EditPotionCB.SelectedItem as Potion;
            var confirmResult = MessageBox.Show("האם אתה בטוח שאתה רוצה למחוק את " +p.Name +"?","מחיקת שיקוי", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                //He is sure so we go ahead.
                alchemist.DeletePotion(p);
                UpdateEditPotionTab();
            }
            else
            {
                //He chickened out
                return;
            }
        }

        //
        //Effect Info tab
        //
        private void UpdateEffectInfoTab()
        {
            EffectInfoCB.Items.Clear();
            EffectInfoAddButton.Enabled = false;
            EffectInfoEffectLV.Items.Clear();
            EffectInfoPotionLV.Items.Clear();
            EffectInfoProductLV.Items.Clear();
            //Get all effects, potions and products
            alchemist.GetAllProducts();
            alchemist.GetAllPotions();
            alchemist.GetAllEffects();
            EffectInfoCB.Items.AddRange(alchemist.Effect_List.ToArray());
            EffectInfoCB.DisplayMember = "Name";
        }

        private void EffectInfoCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EffectInfoCB.SelectedIndex < 0)
            {
                EffectInfoAddButton.Enabled = false;
                return;
            }
            EffectInfoAddButton.Enabled = true;
        }

        private void EffectInfoAddButton_Click(object sender, EventArgs e)
        {
            if (EffectInfoCB.SelectedIndex < 0)
                return;
            Effect effect = EffectInfoCB.SelectedItem as Effect;
            ListViewItem LVI = new ListViewItem(effect.Name);
            LVI.ToolTipText = effect.Description;
            LVI.Tag = effect;
            EffectInfoEffectLV.Items.Add(LVI);
            Button b = new Button();
            b.BackgroundImage = Properties.Resources.red_minus;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            b.BackColor = SystemColors.Control;
            //Add event handler...
            b.Click += new EventHandler(EffectInfoRemoveButton_Click);
            b.Tag = LVI;
            EffectInfoEffectLV.AddEmbeddedControl(b, 1, LVI.Index);
            //Remove the effect from the CB
            EffectInfoCB.Items.Remove(effect);
            //Refresh Data on the other Listviews
            EffectInfoRefresh();

        }

        private void EffectInfoRemoveButton_Click(object sender, EventArgs e)
        {
            Effect effect = ((sender as Button).Tag as ListViewItem).Tag as Effect;
            //Add the effect into the CB
            EffectInfoCB.Items.Add(effect);
            //Remove the item and the button from the Listview
            EffectInfoEffectLV.RemoveEmbeddedControl(sender as Button);
            EffectInfoEffectLV.Items.Remove(((sender as Button).Tag as ListViewItem));
            EffectInfoRefresh();
        }

        private void EffectInfoRefresh()
        {
            EffectInfoProductLV.Items.Clear();
            EffectInfoPotionLV.Items.Clear();
            if (EffectInfoEffectLV.Items.Count < 1)
                return;
            List<Effect> effect_list = new List<Effect>();
            foreach (ListViewItem item in EffectInfoEffectLV.Items)
            {
                effect_list.Add(item.Tag as Effect);
            }
            //Now we'll need to find all the potions and products that all of the effects in effect_list are in.
            ListViewItem LVI;
            List<Potion> pot_list = alchemist.FindPotionsWithEffects(effect_list);
            foreach (Potion pot in pot_list)
            {
                LVI = new ListViewItem(pot.Name);
                LVI.ToolTipText = pot.ToString();
                EffectInfoPotionLV.Items.Add(LVI);
            }
            List<Product> prod_list = alchemist.FindProductsWithEffects(effect_list);
            foreach (Product prod in prod_list)
            {
                LVI = new ListViewItem(prod.GetProdName());
                LVI.ToolTipText = "|";
                foreach (Effect e in prod.Effects)
                    LVI.ToolTipText += " " + e.Name + " |";
                EffectInfoProductLV.Items.Add(LVI);
            }
        }

        //
        //Edit Special Materials Tab
        //

        private void UpdateEditSpecialTab()
        {
            //Remove and refill the Tab with data
            EditSpecialsCB.Items.Clear();
            EditSpecialsNameTB.Text = "";
            EditSpecialsCommentTB.Text = "";
            EditSpecialOriginEffectCB.Items.Clear();
            EditSpecialDestEffectCB.Items.Clear();
            EditSpecialTransmuteLV.Items.Clear();
            //Make sure our alchemist is aware of all the most updated specials.
            alchemist.GetAllSpecials();
            //We also need the effects to be updated
            alchemist.GetAllEffects();
            EditSpecialsCB.Items.AddRange(alchemist.Special_List.ToArray());
            //Add the "New Special" option
            Special spec = new Special();
            spec.Name = "<רכיב מיוחד חדש>";
            EditSpecialsCB.Items.Add(spec);
            EditSpecialsCB.DisplayMember = "Name";
        }

        private void EditSpecialsCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelectedSpecial();   
        }

        private void RefreshSelectedSpecial()
        {
            if (EditSpecialsCB.SelectedIndex < 0)
            {
                return;
            }
            //Empty the controls
            EditSpecialOriginEffectCB.Items.Clear();
            EditSpecialDestEffectCB.Items.Clear();
            EditSpecialTransmuteLV.Items.Clear();
            Special selected_spec = EditSpecialsCB.SelectedItem as Special;
            ListViewItem LVI;
            Button b;
            Effect orig_effect, dest_effect;
            if (selected_spec.ID < 0)
            {
                EditSpecialsNameTB.Text = "";
            }
            else
            {
                EditSpecialsNameTB.Text = selected_spec.Name;
            }
            EditSpecialsCommentTB.Text = selected_spec.Comment;
            //Add all the effects that are in the special to the LV, the rest to the origeffectCB
            foreach (Effect e in alchemist.Effect_List)
            {
                orig_effect = selected_spec.Effect_Pairs.Where(pair => pair.Key.ID == e.ID).Select(pair => pair.Key).FirstOrDefault();
                if (orig_effect == null)
                {
                    //Meaning the ID is not in the Special, so we add it to the CB
                    EditSpecialOriginEffectCB.Items.Add(e);
                }
                else
                {
                    //Meaning it is in the special, so we add it to the LV.
                    LVI = new ListViewItem(orig_effect.Name);
                    //Get dest effect
                    dest_effect = selected_spec.Effect_Pairs[orig_effect];
                    //Write a longer tooltip to include both of the effects.
                    LVI.ToolTipText = orig_effect.Name + ": " + orig_effect.Description + Environment.NewLine + dest_effect.Name + ": " + dest_effect.Description;
                    LVI.SubItems.Add(new ListViewItem.ListViewSubItem(LVI, dest_effect.Name));
                    EditSpecialTransmuteLV.Items.Add(LVI);
                    //Removal Button
                    b = new Button();
                    b.BackgroundImage = Properties.Resources.red_minus;
                    b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                    b.BackColor = SystemColors.Control;
                    //Add event handler...
                    b.Click += new EventHandler(EditSpecialRemoveFromLV_Click);
                    b.Tag = orig_effect;
                    EditSpecialTransmuteLV.AddEmbeddedControl(b, 2, LVI.Index);
                }
            }
            EditSpecialOriginEffectCB.DisplayMember = "Name";
        }

        private void EditSpecialRemoveFromLV_Click(object sender, EventArgs e)
        {
            Effect orig = (sender as Button).Tag as Effect;
            Special spec = orig.Owner as Special;
            alchemist.RemovePairFromSpecial(orig, spec);
            RefreshSelectedSpecial();
        }

        private void EditSpecialOriginEffectCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditSpecialOriginEffectCB.SelectedIndex < 0 || EditSpecialsCB.SelectedIndex < 0)
            {
                return;
            }
            EditSpecialDestEffectCB.Items.Clear();
            EditSpecialDestEffectCB.Items.AddRange(alchemist.Effect_List.ToArray());
            EditSpecialDestEffectCB.Items.Remove(EditSpecialOriginEffectCB.SelectedItem);
            EditSpecialDestEffectCB.DisplayMember = "Name";
            
        }

        private void EditSpecialSaveButton_Click(object sender, EventArgs e)
        {
            if (EditSpecialsCB.SelectedIndex < 0)
                return;
            int index = -1;
            Special selected_spec = EditSpecialsCB.SelectedItem as Special;
            selected_spec.Name = EditSpecialsNameTB.Text;
            selected_spec.Comment = EditSpecialsCommentTB.Text;
            if (selected_spec.ID >= 0)
            {
                alchemist.UpdateSpecial(selected_spec);
            }
            else
            {
                alchemist.AddNewSpecial(selected_spec);
            }
            UpdateEditSpecialTab();
            index = alchemist.Special_List.FindIndex(item => item.Name == selected_spec.Name);
            EditSpecialsCB.SelectedIndex = index;
        }

        private void EditSpecialAddTransmuteButton_Click(object sender, EventArgs e)
        {
            if (EditSpecialOriginEffectCB.SelectedIndex < 0 || EditSpecialsCB.SelectedIndex < 0 || EditSpecialDestEffectCB.SelectedIndex < 0)
            {
                return;
            }
            int index = -1;
            Special selected_spec = EditSpecialsCB.SelectedItem as Special;
            Effect orig_effect = EditSpecialOriginEffectCB.SelectedItem as Effect;
            Effect dest_effect = EditSpecialDestEffectCB.SelectedItem as Effect;
            if (selected_spec.ID >= 0)
            {
                alchemist.AddPairToSpecial(selected_spec, orig_effect, dest_effect);
            }
            else
            {
                //Add a new special
                selected_spec.Name = EditSpecialsNameTB.Text;
                selected_spec.Comment = EditSpecialsCommentTB.Text;
                alchemist.AddNewSpecial(selected_spec);
                //save the index in the combobox
                index = EditSpecialsCB.SelectedIndex;
                UpdateEditSpecialTab();
                //go back to index
                EditSpecialsCB.SelectedIndex = index;
                selected_spec = EditSpecialsCB.SelectedItem as Special;
                //Add transmutation
                alchemist.AddPairToSpecial(selected_spec, orig_effect, dest_effect);
            }
            RefreshSelectedSpecial();
        }

        //
        //Power Levels Tab
        //
        private void UpdatePowerLevelTab()
        {
            //Update Alchemist
            alchemist.GetAllEffects();
            alchemist.GetAllSpecials();
            //Clear Listviews
            UpdatePowerEffectLV.Items.Clear();
            UpdatePowerSpecialLV.Items.Clear();
            //Sort list by power. normal sort so the lowest will be first (which makes it the most powerful)
            alchemist.Effect_List.Sort((x, y) => x.Power.CompareTo(y.Power));
            alchemist.Special_List.Sort((x, y) => x.Power.CompareTo(y.Power));
            //Insert values into the listviews
            ListViewItem LV;
            foreach (Effect e in alchemist.Effect_List)
            {
                LV = new ListViewItem(e.Name);
                LV.Tag = e;
                LV.ToolTipText = e.Description;
                UpdatePowerEffectLV.Items.Add(LV);
            }
            foreach (Special s in alchemist.Special_List)
            {
                LV = new ListViewItem(s.Name);
                LV.Tag = s;
                UpdatePowerSpecialLV.Items.Add(LV);
            }

        }

        enum Direction { UP = -1, DOWN = +1 };

        //this moves the selected item in the Listview up or down.
        void ListViewMove(ListView lv, Direction direction)
        {
            if (lv.SelectedItems.Count > 0)
            {
                int selIdx = lv.SelectedItems[0].Index;
                ListViewItem tmp = lv.Items[selIdx];

                if (((selIdx != 0) && direction == Direction.UP) ||
                    ((selIdx != lv.Items.Count - 1) && (direction == Direction.DOWN)))
                {
                    lv.Items.RemoveAt(selIdx);
                    tmp = lv.Items.Insert(selIdx + (int)direction, tmp);
                    tmp.Selected = true;
                }
            }
            lv.Focus();
        }

        private void EffectPowerUpButton_Click(object sender, EventArgs e)
        {
            CheckUp();
            if (UpdatePowerEffectLV.SelectedItems.Count != 1)
            {
                return;
            }
            if (UpdatePowerEffectLV.SelectedItems[0].Index > 0)
            {
                //Only do something if this is not the first item in the list
                Effect selected_effect = UpdatePowerEffectLV.SelectedItems[0].Tag as Effect;
                Effect target_effect = UpdatePowerEffectLV.Items[UpdatePowerEffectLV.SelectedItems[0].Index - 1].Tag as Effect;
                //Switch the powers of the effects.
                int temp = 0;
                temp = selected_effect.Power;
                selected_effect.Power = target_effect.Power;
                target_effect.Power = temp;
                alchemist.UpdateEffect(selected_effect);
                alchemist.UpdateEffect(target_effect);
                ListViewMove(UpdatePowerEffectLV, Direction.UP);
            }
            else
            {
                UpCounter++;
            }
            
        }

        private void EffectPowerDownButton_Click(object sender, EventArgs e)
        {
            if (UpdatePowerEffectLV.SelectedItems.Count != 1)
                return;
            if (UpdatePowerEffectLV.SelectedItems[0].Index < UpdatePowerEffectLV.Items.Count - 1)
            {
                //Only do something if this is not the last item in the list
                Effect selected_effect = UpdatePowerEffectLV.SelectedItems[0].Tag as Effect;
                Effect target_effect = UpdatePowerEffectLV.Items[UpdatePowerEffectLV.SelectedItems[0].Index + 1].Tag as Effect;
                //Switch the powers of the effects.
                int temp = 0;
                temp = selected_effect.Power;
                selected_effect.Power = target_effect.Power;
                target_effect.Power = temp;
                alchemist.UpdateEffect(selected_effect);
                alchemist.UpdateEffect(target_effect);
                ListViewMove(UpdatePowerEffectLV, Direction.DOWN);
            }
        }

        private void SpecialPowerUpButton_Click(object sender, EventArgs e)
        {
            CheckUp();
            if (UpdatePowerSpecialLV.SelectedItems.Count != 1)
                return;
            if (UpdatePowerSpecialLV.SelectedItems[0].Index > 0)
            {
                //Only do something if this is not the first item in the list
                Special selected_spec = UpdatePowerSpecialLV.SelectedItems[0].Tag as Special;
                Special target_spec = UpdatePowerSpecialLV.Items[UpdatePowerSpecialLV.SelectedItems[0].Index - 1].Tag as Special;
                //Switch the powers of the effects.
                int temp = 0;
                temp = selected_spec.Power;
                selected_spec.Power = target_spec.Power;
                target_spec.Power = temp;
                alchemist.UpdateSpecial(selected_spec);
                alchemist.UpdateSpecial(target_spec);
                ListViewMove(UpdatePowerSpecialLV, Direction.UP);
            }
            else
            {
                UpCounter++;
            }
        }

        private void SpecialPowerDownButton_Click(object sender, EventArgs e)
        {
            if (UpdatePowerSpecialLV.SelectedItems.Count != 1)
                return;
            if (UpdatePowerSpecialLV.SelectedItems[0].Index < UpdatePowerSpecialLV.Items.Count - 1)
            {
                //Only do something if this is not the first item in the list
                Special selected_spec = UpdatePowerSpecialLV.SelectedItems[0].Tag as Special;
                Special target_spec = UpdatePowerSpecialLV.Items[UpdatePowerSpecialLV.SelectedItems[0].Index + 1].Tag as Special;
                //Switch the powers of the effects.
                int temp = 0;
                temp = selected_spec.Power;
                selected_spec.Power = target_spec.Power;
                target_spec.Power = temp;
                alchemist.UpdateSpecial(selected_spec);
                alchemist.UpdateSpecial(target_spec);
                ListViewMove(UpdatePowerSpecialLV, Direction.DOWN);
            }
        }

        private void CheckUp()
        {
            UpForm uf;
            if (UpCounter > 0 && UpCounter % 10 == 0)
            {
                uf = new UpForm();
                uf.ShowDialog();
            }
        }

        private void UpdateAddPotionTab()
        {
            //Update Alchemist
            alchemist.GetAllIngredients();
            alchemist.GetAllProcedures();
            alchemist.GetAllEffects();
            //Clear the items
            AddPotionEffectLV.Items.Clear();
            AddPotEffectCB.Items.Clear();
            AddPotionNameCB.Clear();
            AddPotionCreatorTB.Clear();
            AddPotionDescriptionTB.Clear();
            //Refill the CB
            AddPotEffectCB.Items.AddRange(alchemist.Effect_List.ToArray());
            AddPotEffectCB.DisplayMember = "Name";
            AddPotSubmitButton.Enabled = false; //This activates once the potion has 3 effects and a name.
        }

        private void AddPotEffectCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //If the potion has 5 effects, this will do nothing
            if (AddPotEffectCB.SelectedIndex < 0 || AddPotionEffectLV.Items.Count >= 5)
                return;
            Effect effect = AddPotEffectCB.SelectedItem as Effect;
            //Add to LV
            ListViewItem LVI = new ListViewItem(effect.Name);
            LVI.ToolTipText = effect.Description;
            LVI.Tag = effect;
            AddPotionEffectLV.Items.Add(LVI);
            Button b = new Button();
            b.BackgroundImage = Properties.Resources.red_minus;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            b.BackColor = SystemColors.Control;
            //Add event handler...
            b.Click += new EventHandler(AddPotionEffectRemoveButton_Click);
            b.Tag = LVI;
            AddPotionEffectLV.AddEmbeddedControl(b, 1, LVI.Index);
            //Remove the effect from the CB
            AddPotEffectCB.Items.Remove(effect);
            //Refresh Data on the other Listviews
            AddPotionRefresh();
        }

        private void AddPotionEffectRemoveButton_Click(object sender, EventArgs e)
        {
            Effect effect = ((sender as Button).Tag as ListViewItem).Tag as Effect;
            //Add the effect into the CB
            AddPotEffectCB.Items.Add(effect);
            //Remove the item and the button from the Listview
            AddPotionEffectLV.RemoveEmbeddedControl(sender as Button);
            AddPotionEffectLV.Items.Remove(((sender as Button).Tag as ListViewItem));
            AddPotionRefresh();
        }

        private void AddPotionNameCB_TextChanged(object sender, EventArgs e)
        {
            AddPotionRefresh();
        }

        private void AddPotionRefresh()
        {
            if (AddPotionEffectLV.Items.Count >= 3 && AddPotionNameCB.Text != "")
                AddPotSubmitButton.Enabled = true;
            else
                AddPotSubmitButton.Enabled = false;
        }

        private void AddPotSubmitButton_Click(object sender, EventArgs e)
        {
            Potion p = new Potion();
            p.Name = AddPotionNameCB.Text;
            p.Creator = AddPotionCreatorTB.Text;
            p.Description = AddPotionDescriptionTB.Text;
            p.Effects = new List<FinalEffect>();
            foreach (ListViewItem item in AddPotionEffectLV.Items)
            {
                Effect effect = item.Tag as Effect;
                FinalEffect fe = new FinalEffect();
                fe.Name = effect.Name;
                fe.Level = effect.Level;
                fe.ID = effect.ID;
                p.Effects.Add(fe);
            }
            alchemist.AddNewPotion(p);
            UpdateAddPotionTab();
        }
    }
}
