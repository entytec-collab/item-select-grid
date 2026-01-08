README

How to set:
1- Add an empty GameObject with TabsItemSelectorManager and UIBuilder components
2- Add TabsController component to the content children of the tabs ScrollView GameObject (Has a ScrollRect component)
3- Add SectionsController component to the content children of the sections ScrollView GameObject (Has a ScrollRect component) 
4- Assign references to those three objects
5- Create SectionPrefab, ItemPrefab and TabPrefab as described below in the Set Prefabs topic.

Settings adjustments:
 - In SectionsController component define:
  -- Columns number
  -- Tab Change tolerance: increase to make the selected category button change before the category reaches to the very top of the content. 

Set Prefabs:
 - Section Prefab: add SectionView component
 - ItemPrefab: add ItemCellView
   -- Asign the button or toggle component of the prefab

Tips:
1- You can créate multiple item selectors in the same scene, just créate a manager for each and assign components and references accordingly

Tabs Prefab:
- Add Button component to use tabs as navigators
- Add Toogle component to use tabs as filters (Tip: you can separately add a select/Deselect all option)

Tabs Layout Settings:
- Use Force Expand Child to always fit the tabs to a fixed UI (content) size, otherwise scroll will automatically activate.
  -- Content Size Fitter and Layout Groups are added in code, don't add manually.
- Don't add LayoutElement parameters:
  - This is to avoid unreachable tabs outside of content size 
  - For now Tabs don't accept to only Control Child Size, it automatically Forces Expand too and viceversa. 


Sections:
- Uses sections prefab that requires:
  -- Children GameObject with a GridLayoutGroup
  -- Optional: Add children GameObject with a TextMeshPro to use as label automatically
    -- Caution: 
      -- The first children with TextMeshPro Will be used as label for the inline labels and for tabs
      -- Don't add more than one TextMeshPro in the label GameObject