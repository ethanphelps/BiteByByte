import { Category } from "@app/_models/category";

/*
 * Extends the Category class by adding a reference to the parent category
 */
export class FocusArea extends Category{
  category: string; // reference to encapsulating Category should be of type ??
}
