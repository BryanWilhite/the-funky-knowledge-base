module.exports = function(config) {
    config.addCollection('entries', collection => {
      return collection.getFilteredByGlob('entry/*.md');
    });

    config.addFilter("slice", require("./filters/slice.js"));
    config.addFilter("lookup", require("./filters/lookup.js"));
    config.addPassthroughCopy("images");

    return {
        dir: {
          input: ".",
          output: "docs",
          includes: "templates"
        },
        htmlTemplateEngine : "liquid",
        markdownTemplateEngine : "liquid",
        templateFormats : ["html", "md", "css"]
      };
};
